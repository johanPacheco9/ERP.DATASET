using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ISupplier;
public interface ISupplierService
{
    Task<Proveedor> AddProveedorAsync(Proveedor proveedor, CancellationToken cancellationToken);
    Task<bool> DeleteProveedorById(int id, CancellationToken cancellationToken);
    Task<Proveedor> UpdateProveedor(int id, CancellationToken cancellationToken);
    Task<Proveedor?> GetProveedorById(int id, CancellationToken cancellationToken);
}
