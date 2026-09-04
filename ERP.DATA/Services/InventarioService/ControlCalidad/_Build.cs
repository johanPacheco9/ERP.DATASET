using ERP.DATA.Repositories;
using ERP.DATA.Services.InventarioService.Movimientos;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.ControlCalidad;

public partial class ControlCalidadManager(MainDataContext context, ILogger<ControlCalidadManager> logger, MovimientosManager movimientosManager)
{
    private readonly MainDataContext _context = context;
    private readonly ILogger<ControlCalidadManager> _logger = logger;
    private readonly MovimientosManager _movimientosManager = movimientosManager;
}
