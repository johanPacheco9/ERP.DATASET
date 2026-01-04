using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;

namespace ERP.DATA.Services.InventarioService.BodeegaService;

public partial class BodegaService
{
    public async Task<int> AddBodegaAsync(CreateBodegaRequest bodega, CancellationToken cancellationToken)
	{
		try
		{
            var entity = new Bodega
            {
                Codigo = bodega.Code,
                Nombre = bodega.Nombre,
                Ubicacion = bodega.Ubicacion,
                Descripcion = bodega.Descripcion,
                Capacidad_Maxima = bodega.CapacidadMaxima,
                TipoBodega = bodega.TipoBodega,
                StoreId = bodega.storeId,
                CreatedBy = "SYSTEMUSER",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Bodegas.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
		catch (Exception)
		{

			throw;
		}
    }
}



