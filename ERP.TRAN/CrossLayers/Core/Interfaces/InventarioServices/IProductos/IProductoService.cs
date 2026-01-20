using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;


namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;

public interface IProductoService
{
    Task<int> AddProductoAsync(CreateProductoRequest createProductoRequest, CancellationToken cancellationToken);
    Task<bool> DeleteProductoById(int id, CancellationToken cancellationToken);
    Task<Producto> UpdateProducto(int id, CancellationToken cancellationToken);
    Task<ProductoBaseDto?> GetProductoById(int id, CancellationToken cancellationToken);
    Task<PagedList<ProductoSummaryDto>> ListAsync(ListProductRequest request,CancellationToken cancellationToken);
}

