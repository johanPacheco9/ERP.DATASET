using ERP.DATA.Services.UserService;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.ProductoVarianteService;

public partial class ProductVariantService(
    ILogger<ProductVariantService> logger,
    MainDataContext context ,
    UserManager userManager)
{

    private readonly ILogger<ProductVariantService> _logger = logger;
}
