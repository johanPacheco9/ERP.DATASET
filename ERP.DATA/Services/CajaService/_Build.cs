using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager(MainDataContext context, ILogger<CajaManager> logger)
{
    private readonly MainDataContext _context = context;
    private readonly ILogger<CajaManager> _logger = logger;
}