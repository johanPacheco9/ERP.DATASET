using ERP.DATA.Repositories;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;
namespace ERP.DATA.Services.InventarioService.ProductService;

/// <summary>
/// Servicio de gestión de productos
/// </summary>
public partial class ProductService(ILogger<ProductService> logger, MainDataContext context)
{
    private readonly ILogger<ProductService> _logger = logger;
}
