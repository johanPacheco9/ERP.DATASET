using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    public async Task<List<AuditSummaryDto>> ListAudits(CancellationToken cancellationToken = default)
    {
        var audits = await _context.Audit
            .AsNoTracking()
            .Include(a => a.Warehouse)
            .OrderByDescending(a => a.CreatedAt)
            .Take(100)
            .ToListAsync(cancellationToken);

        return audits.Select(a => new AuditSummaryDto(
            a.Id,
            a.StartDate,
            a.EndDate,
            a.Warehouse?.Name,
            a.Type.GetDisplayName(),
            a.Status.GetDisplayName(),
            a.TotalExpectedUnits,
            a.TotalCountedUnits,
            a.TotalMatches,
            a.TotalMissing,
            a.CreatedAt
        )).ToList();
    }
}
