using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.WarehouseService;
public partial class WarehouseService
{
    public async Task<WarehouseDetailDTO> GetBodegaByIdAsync(
   int id,
   CancellationToken cancellationToken)
    {
       var response =  await context.Warehouse
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new WarehouseDetailDTO(
                b.Id,
                b.Code,
                b.Description,
                b.Ubication,
                b.IsActive,
                b.CreatedAt,
                b.UpdatedAt,
                b.Max_Capacity
            ))
            .FirstOrDefaultAsync(cancellationToken);
        if (response != null) 
        {
            return response;
        }
        return null;
    }
}
