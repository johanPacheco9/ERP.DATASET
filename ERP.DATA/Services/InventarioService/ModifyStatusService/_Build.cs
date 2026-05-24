using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.ModifyStatusService;
/// <summary>
/// Servicio que se llamará en en otros lados para el cambio de status. estudiar como hacerlo tipo arquitectura por eventos.
/// </summary>
public partial class StatusService
{
    private readonly ILogger<StatusService> _logger;
    private readonly MainDataContext _context;

    public StatusService(ILogger<StatusService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
}