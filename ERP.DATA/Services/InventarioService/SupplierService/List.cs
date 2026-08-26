using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.Inventario.ProveedorService;

public partial class SupplierService
{
    public async Task<List<Proveedor>> ListAsync(int take = 100, CancellationToken cancellationToken = default)
    {
        return await context.Supplier
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
