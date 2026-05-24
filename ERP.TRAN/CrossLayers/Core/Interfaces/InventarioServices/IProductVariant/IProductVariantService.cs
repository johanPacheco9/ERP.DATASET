using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductVariant;

public interface IProductVariantService
{
    Task<List<int>>AddProductoVariantes(List<CreateProductoVarianteRequest> request, CancellationToken cancellationToken);
   
}
