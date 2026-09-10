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
    
    private Result<UserDetailDto>? _usuarioAutenticadoCache;

    public async Task<Result<UserDetailDto>> GetUserAuthenticate()
    {
        if (_usuarioAutenticadoCache is not null)
        {
            return _usuarioAutenticadoCache;
        }

        var userId = await GetUserId();
        if (userId is null)
        {
            return Result<UserDetailDto>.Failure(Error.Failure("Unauthorized", "Usuario no autenticado."));
        }

        var resultado = await GetById(userId.Value);
        _usuarioAutenticadoCache = resultado;

        return resultado;
    }
}