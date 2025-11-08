using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;

public interface IBodegaService
{
    Task<bool> BodegaExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> AddBodegaAsync(Bodega bodega, CancellationToken cancellationToken);
    Task<bool> ExisteBodegaPorCodigoAsync(string codigo, CancellationToken cancellationToken);
}