using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ISupplier;
public interface ISupplierService
{
    Task<Supplier> AddProveedorAsync(Supplier proveedor, CancellationToken cancellationToken);
    Task<bool> DeleteProveedorById(int id, CancellationToken cancellationToken);
    Task<Supplier> UpdateProveedor(int id, CancellationToken cancellationToken);
    Task<Supplier?> GetProveedorById(int id, CancellationToken cancellationToken);
}
