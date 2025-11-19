using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.BodeegaService;

public partial class BodegaService : IBodegaService
{
    private readonly ILogger<BodegaService> _logger;
    private readonly MainDataContext _context;

    public BodegaService(ILogger<BodegaService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task<bool> BodegaExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExisteBodegaPorCodigoAsync(string codigo, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Bodega> GetBodegaByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}


