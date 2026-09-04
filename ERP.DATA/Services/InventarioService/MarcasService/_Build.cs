using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.MarcasService;

public partial class MarcasManager(MainDataContext context, ILogger<MarcasManager> logger)
{
    private readonly MainDataContext _context = context;
    private readonly ILogger<MarcasManager> _logger = logger;
}