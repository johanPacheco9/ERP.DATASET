namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    /// <summary>
    ///     Registra una o varias unidades físicamente encontradas durante la auditoría.
    ///     Actualiza el estado del UnitProductAudit correspondiente y los contadores del Audit.
    /// </summary>
    public async Task<RegisterFoundUnitResultDto> RegisterFoundUnits(
        RegisterFoundUnitsRequest request,
        CancellationToken cancellationToken)
    {
        var audit = await _context.Audit
            .FirstOrDefaultAsync(a => a.Id == request.AuditId, cancellationToken);

        if (audit == null)
            throw new InvalidOperationException($"La auditoría {request.AuditId} no existe.");

        if (audit.Status == AuditStatus.Completada || audit.Status == AuditStatus.Cancelada)
            throw new InvalidOperationException(
                $"No se pueden registrar unidades en una auditoría con estado '{audit.Status.GetDisplayName()}'.");

        // Activar auditoría si estaba pendiente
        if (audit.Status == AuditStatus.Pendiente) audit.Status = AuditStatus.EnProgreso;

        var results = new List<FoundUnitResultItemDto>();

        foreach (var serial in request.Serials)
        {
            var unitAudit = await _context.UnitProductAudits
                .FirstOrDefaultAsync(u =>
                        u.AuditId == request.AuditId &&
                        u.Serial == serial,
                    cancellationToken);

            if (unitAudit == null)
            {
                // El serial no pertenece a esta auditoría → se trata como sobrante
                results.Add(new FoundUnitResultItemDto(
                    serial,
                    success: false,
                    message:
                    "Serial no encontrado en la auditoría. Use RegisterSurplusUnit para registrarlo como sobrante."));
                continue;
            }

            if (unitAudit.Status == UnitProductAuditStatus.Found ||
                unitAudit.Status == UnitProductAuditStatus.LocationMismatch ||
                unitAudit.Status == UnitProductAuditStatus.StatusMismatch)
            {
                results.Add(new FoundUnitResultItemDto(
                    serial,
                    success: false,
                    message: "Esta unidad ya fue registrada anteriormente."));
                continue;
            }

            // Comparar bodega física vs bodega en BD
            var hasLocationMismatch = request.PhysicalWarehouseId.HasValue &&
                                      unitAudit.BodegaId != request.PhysicalWarehouseId.Value;

            unitAudit.Status = hasLocationMismatch
                ? UnitProductAuditStatus.LocationMismatch
                : UnitProductAuditStatus.Found;

            unitAudit.PhysicalWarehouseId = request.PhysicalWarehouseId;
            unitAudit.CountedAt = DateTime.UtcNow;
            unitAudit.CountedBy = request._AuditorAuth0Id;
            unitAudit.UpdatedAt = DateTime.UtcNow;

            // Actualizar contadores del Audit
            audit.TotalCountedUnits++;
            if (hasLocationMismatch)
                audit.TotalLocationDifferences++;
            else
                audit.TotalMatches++;

            results.Add(new FoundUnitResultItemDto(serial, success: true,
                message: hasLocationMismatch
                    ? "Registrada con diferencia de ubicación."
                    : "Registrada correctamente."));
        }

        // Recalcular faltantes
        audit.TotalMissing = audit.TotalExpectedUnits - audit.TotalCountedUnits;

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterFoundUnitResultDto(
            request.AuditId,
            TotalProcessed: request.Serials.Count,
            Successful: results.Count(r => r.Success),
            Failed: results.Count(r => !r.Success),
            Items: results);
    }
}