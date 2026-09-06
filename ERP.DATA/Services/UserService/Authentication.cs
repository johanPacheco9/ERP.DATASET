using System.Security.Claims;
using ERP.TRAN.CrossLayers.API.Users.Enums;

namespace ERP.DATA.Services.UserService;

public partial class UserManager
{
    public async Task<int?> GetUserId()
    {
        var claim = await ObtenerClaims(ClaimTypes.NameIdentifier);
        return claim is not null && int.TryParse(claim, out var id) ? id : null;
    }

    public async Task<UserRole?> GetRole()
    {
        var claim = await ObtenerClaims(ClaimTypes.Role);
        return claim is not null && Enum.TryParse<UserRole>(claim, out var role) ? role : null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var usuario = await ObtenerUsuario();
        return usuario.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Sin parámetro devuelve todas las claims del usuario actual.
    /// Con claimType, devuelve solo las que coincidan con ese tipo.
    /// </summary>
    public async Task<IEnumerable<Claim>> GetClaims(string? claimType = null)
    {
        var usuario = await ObtenerUsuario();
        return claimType is null
            ? usuario.Claims
            : usuario.FindAll(claimType);
    }

    private async Task<string?> ObtenerClaims(string claimType)
    {
        var usuario = await ObtenerUsuario();
        return usuario.FindFirst(claimType)?.Value;
    }

    private async Task<ClaimsPrincipal> ObtenerUsuario()
    {
        var usuarioHttp = httpContextAccessor.HttpContext?.User;
        if (usuarioHttp?.Identity?.IsAuthenticated == true)
        {
            return usuarioHttp;
        }

        var estado = await authenticationStateProvider.GetAuthenticationStateAsync();
        return estado.User;
    }
}