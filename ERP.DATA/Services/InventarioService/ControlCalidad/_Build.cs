using ERP.DATA.Repositories;
using ERP.DATA.Services.InventarioService.Movimientos;
using ERP.DATA.Services.UserService;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.ControlCalidad;

public partial class ControlCalidadManager(
    MainDataContext _context,
    ILogger<ControlCalidadManager> _logger,
    MovimientosManager _movimientosManager,
    UserManager userManager)
{
}
