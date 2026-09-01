using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

namespace ERP.DATA.Services.InventarioService.WarehouseService;

public partial class WarehouseService
{
    public async Task<int> AddBodegaAsync(CreateBodegaRequest bodega, CancellationToken cancellationToken)
	{
		try
		{
            var entity = new Warehouse
            {
                Code = bodega.Code,
                Name = bodega.Nombre,
                Ubication = bodega.Ubicacion,
                Description = bodega.Descripcion,
                Max_Capacity = bodega.CapacidadMaxima,
                Type = bodega.TipoBodega,
                StoreId = bodega.storeId,
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            };

            await context.Warehouse.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
		catch (Exception)
		{

			throw;
		}
    }
}



