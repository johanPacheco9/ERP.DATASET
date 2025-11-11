
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovimientos;

public interface IMovimientoService
{
    Task<bool> RegistrarEntradaAsync(Movimiento entrada, CancellationToken cancellationToken);

    Task<bool> RegistrarSalidaAsync(Movimiento salida, CancellationToken cancellationToken);

    Task<bool> RegistrarTranspasoEntreBodegas(Movimiento Origen, Movimiento Destino, CancellationToken cancellationToken);
    Task<bool> MovimientoExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<Movimiento> GetMovimientoByIdAsync(Guid id, CancellationToken cancellationToken);
}

