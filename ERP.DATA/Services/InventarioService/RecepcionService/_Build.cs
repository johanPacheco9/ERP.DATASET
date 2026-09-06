using ERP.DATA.Repositories;
using ERP.DATA.Services.UserService;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.RecepcionService;

public partial class RecepcionCompraManager(
    MainDataContext _context,
    ILogger<RecepcionCompraManager> _logger,
    UserManager userManager)
{
}
