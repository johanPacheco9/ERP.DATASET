using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService   
{
     /// <summary>
    /// Retorna el estado de avance de la auditoría: cuántas unidades fueron contadas,
    /// faltan, sobran y el porcentaje de completitud.
    /// </summary>
    public async Task<AuditProgressDto> GetAuditProgress(
        int auditId,
        CancellationToken cancellationToken)
    {
        var audit = await _context.Audit
            .Include(a => a.Warehouse)
            .FirstOrDefaultAsync(a => a.Id == auditId, cancellationToken);

        if (audit == null)
            throw new InvalidOperationException($"La auditoría {auditId} no existe.");

        // Conteo en tiempo real desde el detalle
        var unitAudits = await _context.UnitProductAudits
            .Where(u => u.AuditId == auditId)
            .GroupBy(_ => _.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int expected   = unitAudits.Where(g => g.Status != UnitProductAuditStatus.ExcessProduct).Sum(g => g.Count);
        int found      = unitAudits.Where(g => g.Status == UnitProductAuditStatus.Found || g.Status == UnitProductAuditStatus.FoundOnAudit).Sum(g => g.Count);
        int notFound   = unitAudits.FirstOrDefault(g => g.Status == UnitProductAuditStatus.NotFound)?.Count ?? 0;
        int surplus    = unitAudits.FirstOrDefault(g => g.Status == UnitProductAuditStatus.ExcessProduct)?.Count ?? 0;
        int locDiff    = 0; // Location difference not supported in this enum yet
        int statusDiff = unitAudits.FirstOrDefault(g => g.Status == UnitProductAuditStatus.StatusMismatch)?.Count ?? 0;
        int counted    = found + surplus + statusDiff;

        double completionPct = expected > 0
            ? Math.Round((double)counted / expected * 100, 2)
            : 0;

        return new AuditProgressDto(
            AuditId:           audit.Id,
            audit.WarehouseId,
            WarehouseName:     audit.Warehouse?.Name,
            Status:            audit.Status.GetDisplayName(),
            StartDate:         audit.StartDate,
            TotalExpected:     expected,
            TotalCounted:      counted,
            TotalFound:        found,
            TotalNotFound:     notFound,
            TotalSurplus:      surplus,
            LocationDiffs:     locDiff,
            StatusDiffs:       statusDiff,
            CompletionPercent: completionPct);
    }
}