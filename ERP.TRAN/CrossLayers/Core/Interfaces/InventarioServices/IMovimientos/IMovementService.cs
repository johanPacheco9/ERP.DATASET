using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovement;

public interface IMovementService
{
    Task<int> RegistrarEntradaAsync(CreateEntryMovementRequest createEntry, CancellationToken cancellationToken);

    Task<MovimientoDetailDto> RegistrarSalidaAsync(CreateExitMovementRequest exitMovementRequest, CancellationToken cancellationToken);

    Task<bool> RegistrarTranspasoEntreBodegas(Movement Origen, Movement Destino, CancellationToken cancellationToken);
    Task<bool> MovimientoExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<Movement> GetMovimientoByIdAsync(Guid id, CancellationToken cancellationToken);
}

