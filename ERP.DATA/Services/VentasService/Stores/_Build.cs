using ERP.DATA.Repositories;
using ERP.DATA.Services.CajaService;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.VentasService.Stores;

public partial class StoresManager(MainDataContext context, ILogger<CajaManager> logger)
{
    private readonly MainDataContext _context = context;
    private readonly ILogger<CajaManager> _logger = logger;
}