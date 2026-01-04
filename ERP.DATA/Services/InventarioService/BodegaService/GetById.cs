using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.BodeegaService;
public partial class BodegaService
{
    public async Task<BodegaDetailDTO?> GetBodegaByIdAsync(
   int id,
   CancellationToken cancellationToken)
    {
        return await _context.Bodegas
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BodegaDetailDTO(
                b.Id,
                b.Codigo,
                b.Descripcion,
                b.Ubicacion,
                b.IsActive,
                b.CreatedAt,
                b.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
