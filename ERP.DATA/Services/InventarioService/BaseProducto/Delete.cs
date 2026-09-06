using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.BaseProducto;

public partial class ProductoBaseManager
{
    public async Task<bool> DeleteProductoById(int id, CancellationToken cancellationToken = default)
    {
        var producto = await context.ProductoBase.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (producto == null)
            return false;
        context.ProductoBase.Remove(producto);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
