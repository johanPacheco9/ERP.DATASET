
using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.AuditService;
public partial class AuditoriaService(ILogger<AuditoriaService> logger, MainDataContext context)
{
    private readonly ILogger<AuditoriaService> _logger = logger;
    private readonly MainDataContext _context = context;
}
