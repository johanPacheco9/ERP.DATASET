using ERP.DATA.Repositories;
using ERP.DATA.Services.UserService;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.CajaService;

public partial class CajaManager(MainDataContext _context, ILogger<CajaManager> logger, UserManager userManager)
{
}