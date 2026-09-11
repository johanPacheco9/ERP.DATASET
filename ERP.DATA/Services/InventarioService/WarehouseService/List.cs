using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.WarehouseService;

public partial class WarehouseService
{
    public async Task<PagedList<WarehouseSummaryDto>> List(
        ListWarehousesRequest request,
        CancellationToken cancellationToken)
    {
        var query = context.Warehouse
            .AsNoTracking()
            .AsQueryable();

        // Filtro por término de búsqueda (Nombre, Código o Ubicación)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            string term = request.SearchTerm.Trim().ToLower();
            query = query.Where(w => 
                w.Name.ToLower().Contains(term) || 
                w.Code.ToLower().Contains(term) || 
                (w.Ubication != null && w.Ubication.ToLower().Contains(term))
            );
        }

        // Filtro por tienda
        if (request.StoreId.HasValue)
        {
            query = query.Where(s => s.StoreId == request.StoreId.Value);
        }

        // Filtro por estado
        if (request.Status.HasValue)
        {
            query = query.Where(s => s.Status == request.Status.Value);
        }
    
        query = query.OrderBy(w => w.Name);

        var dtoQuery = query.Select(w => new WarehouseSummaryDto(
            w.Id,
            w.Code,
            w.Name,
            w.Ubication,
            w.Type,
            w.StoreId,
            w.Store.Name,
            w.StockProductos.Count,
            w.Max_Capacity,
            w.IsActive
        ));

        return await PagedList<WarehouseSummaryDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }
}