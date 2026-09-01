using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    public async Task<AuditDetailDto?> GetAuditById(int id, CancellationToken cancellationToken)
    {
        var audit = await _context.Audit
            .AsNoTracking()
            .Include(a => a.Warehouse)
            .Include(a => a.CategoriasAuditadas).ThenInclude(auditCategory => auditCategory.Category)
            .Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (audit == null)
            return null;

        return new AuditDetailDto(
            audit.Id,
            audit.StartDate,
            audit.EndDate,
            audit.WarehouseId,
            audit.Warehouse?.Name,
            audit.CategoriasAuditadas.Select(ct => new CategoriaDetailDto(ct.CategoryId,ct.Category.Name,ct.Category.Description,ct.Category.CreatedAt,ct.Category.UpdatedAt)).ToList(),
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
            "Corregir"
        );
    }
}
