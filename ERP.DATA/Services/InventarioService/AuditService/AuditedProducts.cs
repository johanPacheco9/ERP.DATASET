using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    public async Task<List<UnitProductAuditSummaryDto>> ListUnitAuditedProducts(int? auditId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.UnitProductAudits
            .AsNoTracking()
            .Include(u => u.Bodega)
            .Include(u => u.LineaProducto)
            .Include(u => u.Producto)
            .AsQueryable();

        if (auditId.HasValue)
            query = query.Where(u => u.AuditId == auditId.Value);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Take(500)
            .ToListAsync(cancellationToken);

        return items.Select(u => new UnitProductAuditSummaryDto(
            u.Id,
            u.AuditId,
            u.UnitProductId,
            u.ProductoId,
            u.LineaProductoId,
            u.BodegaId,
            u.Serial,
            u.Status.GetDisplayName(),
            u.Bodega?.Name,
            u.LineaProducto?.Name,
            u.Observaciones,
            u.CreatedAt,
            u.UbicacionFisica,
            u.EstadoFisico,
            u.MotivoDiferencia,
            u.RequiereAccionCorrectiva
        )).ToList();
    }
}