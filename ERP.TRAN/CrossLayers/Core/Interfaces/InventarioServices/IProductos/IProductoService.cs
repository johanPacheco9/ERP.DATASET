using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;

public interface IProductoService
{
    Task<Producto> AddProductoAsync(Producto producto, CancellationToken cancellationToken);
    Task<bool> DeleteProductoById(Guid id, CancellationToken cancellationToken);
    Task<Producto> UpdateProducto(Guid id, CancellationToken cancellationToken);
    Task<Producto?> GetProductoById(Guid id, CancellationToken cancellationToken);
    Task<PagedList<ProductoSummaryDto>> ListAsync(ListProductRequest request,CancellationToken cancellationToken);
}

