using ERP.DATA.Services.UserService;
using Microsoft.Extensions.Logging;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.InventarioService.CategoriaService;

public partial class CategoriaService(
    ILogger<CategoriaService> logger, 
    MainDataContext context,
    UserManager userManager)
{
    
}
