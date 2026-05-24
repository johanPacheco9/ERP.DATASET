using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
namespace ERP.DATA.Services.InventarioService.UnitProductService;

public partial class UnitProductService
{
    public async Task<List<UnitProductDetailDto>> GetByBaseCode(GetUnitProductByCodeRequets request)
    {
        var unitProduct = context.Productos.Where(s => s.LineaProducto.Code == request.Code);
        if (!unitProduct.Any())
        {     
            throw new ArgumentException("No se encontraron productos con el código requerido");
        }

        return unitProduct.Select(p => new
            UnitProductDetailDto(
                p.Id,
                p.Serial?? "",
                p.Status,
                p.FechaVencimiento,
                p.LineaProducto.Name,
                p.LineaProducto.ImagenUrl,
                p.LineaProducto.Code,
                p.SKU,
                p.Atributos,
                p.PrecioVenta,
                p.Bodega.Name
            )).ToList<UnitProductDetailDto>();
    }
    
}