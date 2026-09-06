using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;
namespace ERP.DATA.Services.InventarioService.BaseProducto;

/// <summary>
/// Servicio de gestión de productos
/// </summary>
public partial class ProductoBaseManager(
    ILogger<ProductoBaseManager> logger,
    MainDataContext context)
{
    private readonly ILogger<ProductoBaseManager> _logger = logger;
}
