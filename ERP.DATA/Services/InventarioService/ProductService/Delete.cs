using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Services.InventarioService.ProductService;

public partial class ProductService
{
    public async Task<bool> DeleteProductoById(int id, CancellationToken cancellationToken = default)
    {
        var producto = await context.LineaProductos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (producto == null)
            return false;
        context.LineaProductos.Remove(producto);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
