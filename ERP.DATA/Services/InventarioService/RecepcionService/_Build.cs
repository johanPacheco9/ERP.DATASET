using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.RecepcionService;

public partial class RecepcionCompraManager(MainDataContext context, ILogger<RecepcionCompraManager> logger)
{
    private readonly MainDataContext _context = context;
    private readonly ILogger<RecepcionCompraManager> _logger = logger;
}
