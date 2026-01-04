using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores
{
    public interface IProveedorService
    {
        Task<Proveedor> AddProveedorAsync(Proveedor proveedor, CancellationToken cancellationToken);
        Task<bool> DeleteProveedorById(int id, CancellationToken cancellationToken);
        Task<Proveedor> UpdateProveedor(int id, CancellationToken cancellationToken);
        Task<Proveedor?> GetProveedorById(int id, CancellationToken cancellationToken);
    }
}
