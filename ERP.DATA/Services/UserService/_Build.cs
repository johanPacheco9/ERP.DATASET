using ERP.DATA.Repositories;
using ERP.DATA.Services.TokenService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.UserService;

public partial class UserManager(
    IHttpContextAccessor httpContextAccessor,
    TokenManager tokenManager,
    ILogger<UserManager> logger,
    MainDataContext context)
{
    private readonly ILogger<UserManager> _logger = logger;
}