namespace ERP.DATA.Services.Inventario.ProveedorService;

using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public partial class SupplierService
{
    public async Task<Supplier> AddProveedorAsync(Supplier proveedor, CancellationToken cancellationToken)
    {
        proveedor = new Supplier
        {
            Name = proveedor.Name,
            Nit = proveedor.Nit,
            Address = proveedor.Address,
            Phone = proveedor.Phone,
            IsActive = proveedor.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "01"
        };
        context.Supplier.Add(proveedor);
        await context.SaveChangesAsync(cancellationToken);
        return proveedor;
    }
}
