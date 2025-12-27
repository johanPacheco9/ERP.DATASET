using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovimientos;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService : IMovimientoService
{
    private readonly ILogger<MovimientoService> _logger;
    private readonly MainDataContext _context;

    public MovimientoService(ILogger<MovimientoService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public Task<Movimiento> GetMovimientoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> MovimientoExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
