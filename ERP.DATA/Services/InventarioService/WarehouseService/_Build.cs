using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IWarehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.WarehouseService;

public partial class WarehouseService(ILogger<WarehouseService> logger, MainDataContext context)
{
    private readonly ILogger<WarehouseService> _logger = logger;

    public async Task<bool> BodegaExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Warehouse
            .AnyAsync(b => b.Id == id, cancellationToken);
    }

    public Task<bool> ExisteBodegaPorCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}


