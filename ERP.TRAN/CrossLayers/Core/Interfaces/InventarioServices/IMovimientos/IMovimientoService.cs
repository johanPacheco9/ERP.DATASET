using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovimientos;

public interface IMovimientoService
{
    Task<int> RegistrarEntradaAsync(CreateEntryMovementRequest createEntry, CancellationToken cancellationToken);

    Task<MovimientoDetailDto> RegistrarSalidaAsync(CreateExitMovementRequest exitMovementRequest, CancellationToken cancellationToken);

    Task<bool> RegistrarTranspasoEntreBodegas(Movimiento Origen, Movimiento Destino, CancellationToken cancellationToken);
    Task<bool> MovimientoExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<Movimiento> GetMovimientoByIdAsync(Guid id, CancellationToken cancellationToken);
}

