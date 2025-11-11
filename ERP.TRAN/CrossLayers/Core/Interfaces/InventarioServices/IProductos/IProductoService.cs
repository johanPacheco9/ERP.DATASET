using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
public interface IProductoService
{
    Task<Producto> AddProductoAsync(Producto producto, CancellationToken cancellationToken);
    Task<bool> DeleteProductoById(Guid id, CancellationToken cancellationToken);
    Task<Producto> UpdateProducto(Guid id, CancellationToken cancellationToken);
    Task<Producto?> GetProductoById(Guid id, CancellationToken cancellationToken);
}

