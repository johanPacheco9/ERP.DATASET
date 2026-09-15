using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
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
        var query = _context.Store.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim().ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(term) ||
                                     (s.Description != null && s.Description.ToLower().Contains(term)));
        }

        query = query.OrderByDescending(s => s.IsMainStore).ThenBy(s => s.Name);

        var projectionQuery = query.Select(s => new StoreSummaryDto(
            s.Id,
            s.Name,
            s.Description,
            s.IsMainStore,
            s.IsActive,
            s.Type.GetDisplayName(),
            s.Bodegas.Count,
            s.Cajas.Count,
            s.Cajas.Select(c => c.Id).ToList()
        ));

        return await PagedList<StoreSummaryDto>.ToPagedListAsync(
            projectionQuery,
            request.PageNumber,
            request.PageSize
        );
    }
}