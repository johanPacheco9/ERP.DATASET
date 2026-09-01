using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.ProductoBaseService;

public partial class ProductoBaseService
{
    public async Task<BaseProductDto?> GetProductoById(int id, CancellationToken cancellationToken)
    {
        var producto = await context.ProductoBase
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(p => new BaseProductDto(
                p.Id,
                p.Code,
                p.Name,
                p.Description,
                p.Categorias.Select(pc => new CategoryDto(pc.Category.Id, pc.Category.Name)).ToList(),
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