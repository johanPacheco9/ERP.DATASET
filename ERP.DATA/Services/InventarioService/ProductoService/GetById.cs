namespace ERP.DATA.Services.Inventario.ProductoService;

using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;

public partial class ProductoService
{
    public async Task<Producto?> GetProductoById(int id, CancellationToken cancellationToken)
    {
        var response = await _context.Productos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return response;
    }
}
