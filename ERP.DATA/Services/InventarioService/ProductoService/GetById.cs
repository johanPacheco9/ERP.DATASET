namespace ERP.DATA.Services.Inventario.ProductoService;

using ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using Microsoft.EntityFrameworkCore;

public partial class ProductoService
{
    public async Task<ProductoBaseDto?> GetProductoById(int id, CancellationToken cancellationToken)
    {
        var producto = await _context.Productos
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(p => new ProductoBaseDto(
                p.Id,
                p.Codigo,
                p.Nombre,
                p.Descripcion,
                p.CategoriaId,
                p.ProveedorId,
                p.Unidad_Medida,
                p.Imagen_Url,
                p.Tags,
                p.Estado == ProductoEnumStatus.Activo
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return producto;
    }
}