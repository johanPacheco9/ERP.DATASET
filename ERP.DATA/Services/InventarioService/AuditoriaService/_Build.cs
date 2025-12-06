using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAuditorias;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.AuditoriaService;
public partial class AuditoriaService : IAuditoriaService
{
    private readonly ILogger<AuditoriaService> _logger;
    private readonly MainDataContext _context;

    public AuditoriaService(ILogger<AuditoriaService> logger, MainDataContext context)
    {
        _logger = logger;
        _context = context;
    }
}
