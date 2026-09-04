using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.OrdenesDeCompra;

public partial class OrdenesDeCompraManager(MainDataContext context, ILogger<OrdenesDeCompraManager> logger)
{
    private readonly MainDataContext _context = context;    
    private readonly ILogger<OrdenesDeCompraManager> _logger = logger;
}