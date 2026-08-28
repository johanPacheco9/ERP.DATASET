using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.Stores;

public partial class StoresManager
{
    public async Task<PagedList<StoreSummaryDto>> List(
        ListStoresRequest request, 
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        // 1. Construir la consulta base sin seguimiento para mejor rendimiento
        var query = _context.Store.AsNoTracking().AsQueryable();

        // 2. Aplicar filtro de búsqueda si existe
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim().ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(term) || 
                                     (s.Description != null && s.Description.ToLower().Contains(term)));
        }

        // 3. Ordenar por defecto (las principales primero y luego alfabético)
        query = query.OrderByDescending(s => s.IsMainStore).ThenBy(s => s.Name);

        // 4. Proyectar a StoreSummaryDto ANTES de paginar
        var projectionQuery = query.Select(s => new StoreSummaryDto(
            s.Id,
            s.Name,
            s.Description,
            s.IsMainStore,
            s.IsActive,
            s.Bodegas.Count,
            s.Cajas.Count
        ));

        // 5. Ejecutar usando tu método real: ToPagedListAsync
        return await PagedList<StoreSummaryDto>.ToPagedListAsync(
            projectionQuery, 
            request.PageNumber, 
            request.PageSize
        );
    }
}