namespace ERP.DATA.Services.Inventario.ProductoService;

using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;

public partial class ProductoService
{
    public async Task<Producto?> GetProductoById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _context.Productos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return response;
    }
}
