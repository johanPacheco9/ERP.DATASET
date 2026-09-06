using ERP.DATA.Repositories;
using ERP.DATA.Services.UserService;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.OrdenesDeCompra;

public partial class OrdenesDeCompraManager(
    MainDataContext context, 
    ILogger<OrdenesDeCompraManager> logger,
    UserManager userManager)
{
    private readonly MainDataContext _context = context;    
    private readonly ILogger<OrdenesDeCompraManager> _logger = logger;
}