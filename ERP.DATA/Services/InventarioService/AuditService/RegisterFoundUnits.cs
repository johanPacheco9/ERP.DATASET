using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

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

        if (audit.Status == AuditStatus.Completada || audit.Status == AuditStatus.RejectWithinconsistences)
            throw new InvalidOperationException(
                $"No se pueden registrar unidades en una auditoría con estado '{audit.Status.GetDisplayName()}'.");

        // Activar auditoría si estaba pendiente
        if (audit.Status == AuditStatus.Pendiente) audit.Status = AuditStatus.InProgress;

        var results = new List<FoundUnitResultItemDto>();

        foreach (var serial in request.ProductsIds)
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
                    false,
                    "Serial no encontrado en la auditoría. Use RegisterSurplusUnit para registrarlo como sobrante."));
                continue;
            }

            if (unitAudit.Status == UnitProductAuditStatus.Found ||
                unitAudit.Status == UnitProductAuditStatus.StatusMismatch)
            {
                results.Add(new FoundUnitResultItemDto(
                    serial,
                    false,
                    "Esta unidad ya fue registrada anteriormente."));
                continue;
            }

            // Comparar bodega fisica vs bodega en BD
            var hasLocationMismatch = request.PhysicalWarehouseId != 0 &&
                                      unitAudit.BodegaId != request.PhysicalWarehouseId;
            unitAudit.Status = hasLocationMismatch
                ? UnitProductAuditStatus.StatusMismatch
                : UnitProductAuditStatus.Found;
            unitAudit.BodegaEncontrada = request.PhysicalWarehouseId == 0
                ? null
                : request.PhysicalWarehouseId;
            unitAudit.UpdatedAt = DateTime.UtcNow;

            // Actualizar contadores del Audit
            audit.TotalCountedUnits++;
            if (hasLocationMismatch)
                audit.TotalLocationDifferences++;
            else
                audit.TotalMatches++;

            results.Add(new FoundUnitResultItemDto(serial, Success: true,
                Message: hasLocationMismatch
                    ? "Registrada con diferencia de ubicación."
                    : "Registrada correctamente."));
        }

        // Recalcular faltantes
        audit.TotalMissing = audit.TotalExpectedUnits - audit.TotalCountedUnits;

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterFoundUnitResultDto(
            request.AuditId,
            TotalProcessed: request.ProductsIds.Count,
            Successful: results.Count(r => r.Success),
            Failed: results.Count(r => !r.Success),
            Items: results);
    }
}
