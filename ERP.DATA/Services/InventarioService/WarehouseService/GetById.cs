using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;
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
                b.Name,
                b.Description,
                b.Ubication,
                b.IsActive,
                b.CreatedAt,
                b.UpdatedAt,
                b.Max_Capacity,
                b.Code,
                b.Type
            ))
            .FirstOrDefaultAsync(cancellationToken);
        if (response != null) 
        {
            return response;
        }
        return null;
    }
}
