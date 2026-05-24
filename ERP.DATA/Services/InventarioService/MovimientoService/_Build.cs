using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovement;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService(ILogger<MovimientoService> logger, MainDataContext context)
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
