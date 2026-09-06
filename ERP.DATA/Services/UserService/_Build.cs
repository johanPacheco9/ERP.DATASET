using ERP.DATA.Repositories;
using ERP.DATA.Services.TokenService;
using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.UserService;

public partial class UserManager(
    IHttpContextAccessor httpContextAccessor,
    AuthenticationStateProvider authenticationStateProvider,
    TokenManager tokenManager,
    ILogger<UserManager> logger,
    MainDataContext context)
{
    private readonly ILogger<UserManager> _logger = logger;


    public async Task<Result<UserDetailDto>> GetUserAuthenticate()
    {
        var userId = await GetUserId();
        return await GetById(userId.Value);
    }
}