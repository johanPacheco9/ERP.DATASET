namespace ERP.DATA.Services.Inventario.ProveedorService;

using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;

public partial class ProveedorService
{
    public async Task<Proveedor?> GetProveedorById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _context.Proveedores
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return response;
    }
}
