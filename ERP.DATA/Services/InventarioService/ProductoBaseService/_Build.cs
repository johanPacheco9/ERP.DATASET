using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;
namespace ERP.DATA.Services.InventarioService.ProductoBaseService;

/// <summary>
/// Servicio de gestión de productos
/// </summary>
public partial class ProductoBaseService(ILogger<InventarioService.ProductoBaseService.ProductoBaseService> logger, MainDataContext context)
{
    private readonly ILogger<InventarioService.ProductoBaseService.ProductoBaseService> _logger = logger;
}
