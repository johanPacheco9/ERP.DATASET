using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IUnitProduct;

public interface IUnitProductService
{
    Task<PagedList<UnitProductDetailDto>> ListAsync(ListUnitProductRequest request, CancellationToken cancellationToken);
}
