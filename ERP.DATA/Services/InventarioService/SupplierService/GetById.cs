namespace ERP.DATA.Services.Inventario.ProveedorService;

using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using Microsoft.EntityFrameworkCore;

public partial class SupplierService
{
    public async Task<Supplier?> GetProveedorById(int id, CancellationToken cancellationToken)
    {
        var response = await context.Supplier
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return response;
    }
}
