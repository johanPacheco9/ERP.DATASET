
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;

namespace ERP.DATA.Services.InventarioService.WarehouseService;

public partial class WarehouseService
{
    public async Task<WarehouseDetailDTO> UpdateBodega(UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var bodega = context.Warehouse.Where(s=>s.Id == request.Id).FirstOrDefault();
        if (bodega == null)
            throw new InvalidOperationException($"No existe bodega con ID {request.Id}");
        await context.SaveChangesAsync();
        return new WarehouseDetailDTO(bodega.Id, bodega.Name,bodega.Description, bodega.Ubication,bodega.IsActive,bodega.CreatedAt,bodega.UpdatedAt, bodega.Max_Capacity);
    }
}
