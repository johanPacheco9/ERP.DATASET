using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.Movimientos;

public partial class MovimientosManager(ILogger<MovimientosManager> logger, MainDataContext context)
{
    public Task<Movement> GetMovimientoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MovimientoExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
