
using ERP.DATA.Repositories;
using ERP.DATA.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.AuditService;
public partial class AuditoriaService
    (ILogger<AuditoriaService> logger, 
        MainDataContext context,
        UserManager userManager)
{
    private readonly ILogger<AuditoriaService> _logger = logger;
    private readonly MainDataContext _context = context;
}
