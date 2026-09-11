using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.WarehouseService;

public partial class WarehouseService
{
    public async Task<WarehouseDetailDTO> UpdateBodega(UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var bodega = await context.Warehouse
            .Where(s => s.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (bodega == null)
            throw new InvalidOperationException($"No existe bodega con ID {request.Id}");

        // Actualizamos los campos solo si vienen provistos (o según la lógica de tu negocio)
        if (!string.IsNullOrWhiteSpace(request.Name))
            bodega.Name = request.Name;

        if (request.Code != null)
            bodega.Code = request.Code;

        if (request.Description != null)
            bodega.Description = request.Description;

        if (request.Ubication != null)
            bodega.Ubication = request.Ubication;

        if (request.Max_Capacity.HasValue)
            bodega.Max_Capacity = (int)request.Max_Capacity.Value; // Ajusta el tipo si Max_Capacity en tu entidad es int o decimal

        if (request.StoreId.HasValue && request.StoreId.Value > 0)
            bodega.StoreId = request.StoreId.Value;

        bodega.Type = request.Type;
        bodega.UpdatedAt = DateTime.UtcNow;

        context.Warehouse.Update(bodega);
        await context.SaveChangesAsync(cancellationToken);
        
        return new WarehouseDetailDTO(
            bodega.Id, 
            bodega.Name, 
            bodega.Description,
            bodega.Ubication, 
            bodega.IsActive, 
            bodega.CreatedAt, 
            bodega.UpdatedAt, 
            bodega.Max_Capacity,
            bodega.Code,
            bodega.Type
        );
    }
}