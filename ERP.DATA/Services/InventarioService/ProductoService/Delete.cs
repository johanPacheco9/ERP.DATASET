namespace ERP.DATA.Services.Inventario.ProductoService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class ProductoService
{
    public async Task<bool> DeleteProductoById(Guid id, CancellationToken cancellationToken = default)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (producto == null)
            return false;
        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
