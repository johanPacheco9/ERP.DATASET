using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores
{
    public interface IProveedorService
    {
        Task<Proveedor> AddProveedorAsync(Proveedor proveedor, CancellationToken cancellationToken);
        Task<bool> DeleteProveedorById(Guid id, CancellationToken cancellationToken);
        Task<Proveedor> UpdateProveedor(Guid id, CancellationToken cancellationToken);
        Task<Proveedor?> GetProveedorById(Guid id, CancellationToken cancellationToken);
    }
}
