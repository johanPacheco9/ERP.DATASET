using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Services.InventarioService.ProductService;

public partial class ProductService
{
    public async Task<BaseProductDto?> GetProductoById(int id, CancellationToken cancellationToken)
    {
        var producto = await context.LineaProductos
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(p => new BaseProductDto(
                p.Id,
                p.Code,
                p.Name,
                p.Description,
                p.CategoryId,
                p.SupplierId,
                p.UnidadMedida,
                p.ImagenUrl,
                p.Tags,
                p.IsActive
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return producto;
    }
}