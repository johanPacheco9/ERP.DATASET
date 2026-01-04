

using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductosVariantes;

public interface IProductoVarianteService
{
    Task<List<int>>AddProductoVariantes(List<CreateProductoVarianteRequest> request, CancellationToken cancellationToken);
   
}
