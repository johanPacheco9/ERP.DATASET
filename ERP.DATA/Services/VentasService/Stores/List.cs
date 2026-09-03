using ERP.TRAN.CrossLayers.API.Stores.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums; // GetDisplayName()
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.Stores;
//buscar las tiendas
public partial class StoresManager
{
    public Task<List<StoreSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
        => ListAsync(null, cancellationToken);

    public async Task<List<StoreSummaryDto>> ListAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = _context.Store
            .AsNoTracking()
            .Include(s => s.Bodegas)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(st => st.Name.ToLower().Contains(s));
        }

        var stores = await query
            .OrderByDescending(st => st.IsMainStore)
            .ThenBy(st => st.Name)
            .ToListAsync(cancellationToken);

        return stores
            .Select(st => new StoreSummaryDto(
                st.Id,
                st.Name,
                st.Description,
                st.IsMainStore,
                st.Type.GetDisplayName(),
                st.Bodegas.Count))
            .ToList();
    }

    public async Task<StoreSummaryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var store = await _context.Store
            .AsNoTracking()
            .Include(s => s.Bodegas)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (store == null)
            return null;

        return new StoreSummaryDto(
            store.Id,
            store.Name,
            store.Description,
            store.IsMainStore,
            store.Type.GetDisplayName(),
            store.Bodegas.Count);
    }
}