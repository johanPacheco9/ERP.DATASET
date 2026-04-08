using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;

namespace  Erp.Data.Services.WarehouseService;
public partial class AuditoriaService
{
 /// <summary>
    /// Cierra la auditoría, calcula los totales finales y registra las conclusiones.
    /// Las unidades que permanecen en NotFound se contabilizan como faltantes.
    /// </summary>
    public async Task<AuditDetailDto> CloseAudit(
        CloseAuditRequest request,
        CancellationToken cancellationToken)
    {
        var audit = await _context.Audit
            .Include(a => a.Warehouse)
            .Include(a => a.Category)
            .Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.Id == request.AuditId, cancellationToken);

        if (audit == null)
            throw new InvalidOperationException($"La auditoría {request.AuditId} no existe.");

        if (audit.Status == AuditStatus.Completada)
            throw new InvalidOperationException("La auditoría ya fue cerrada.");

        if (audit.Status == AuditStatus.Cancelada)
            throw new InvalidOperationException("No se puede cerrar una auditoría cancelada.");

        // Calcular totales finales desde la tabla de detalle (fuente de verdad)
        var unitAudits = await _context.UnitProductAudits
            .Where(u => u.AuditId == request.AuditId)
            .ToListAsync(cancellationToken);

        audit.TotalExpectedUnits  = unitAudits.Count(u => u.Status != UnitProductAuditStatus.Surplus);
        audit.TotalCountedUnits   = unitAudits.Count(u => u.Status != UnitProductAuditStatus.NotFound);
        audit.TotalMatches        = unitAudits.Count(u => u.Status == UnitProductAuditStatus.Found);
        audit.TotalMissing        = unitAudits.Count(u => u.Status == UnitProductAuditStatus.NotFound);
        audit.TotalSurplus        = unitAudits.Count(u => u.Status == UnitProductAuditStatus.Surplus);
        audit.TotalLocationDifferences = unitAudits.Count(u => u.Status == UnitProductAuditStatus.LocationMismatch);
        audit.TotalStatusDifferences   = unitAudits.Count(u => u.Status == UnitProductAuditStatus.StatusMismatch);

        audit.Status       = AuditStatus.Completada;
        audit.EndDate      = DateTime.UtcNow;
        audit.Conclusions  = request.Conclusions;
        audit.UpdatedAt    = DateTime.UtcNow;
        audit.UpdatedBy    = request._CloserAuth0Id;

        await _context.SaveChangesAsync(cancellationToken);

        return new AuditDetailDto(
            audit.Id,
            audit.StartDate,
            audit.EndDate,
            audit.Warehouse?.Name,
            audit.Category?.Name,
            audit.ProductId,
            audit.Product?.Name,
            audit.Type.GetDisplayName(),
            audit.Status.GetDisplayName(),
            audit.ResponsibleId,
            audit.SupervisorId,
            audit.TotalExpectedUnits,
            audit.TotalCountedUnits,
            audit.TotalMatches,
            audit.TotalMissing,
            audit.TotalSurplus,
            audit.TotalLocationDifferences,
            audit.TotalStatusDifferences,
            audit.Observations,
            audit.Conclusions,
            audit.CreatedAt,
            audit.CreatedBy);
    }   
}