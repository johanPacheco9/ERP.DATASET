using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.UserService;

public partial class UserManager
{
    public async Task<Result<IniciarSesionResponseDto>> IniciarSesion(IniciarSesionRequest request)
    {
        return await Result<IniciarSesionResponseDto>.TryAsync(async () =>
        {
            var usuarioDb = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == request.Usuario && u.IsActive);
 
            if (usuarioDb is null)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
            }
            
            var hasher = new PasswordHasher<object>();
            var resultadoVerificacion = hasher.VerifyHashedPassword(null!, usuarioDb.PasswordHash, request.Password);
 
            if (resultadoVerificacion == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");
            }
 
            var token = tokenManager.GenerarToken(usuarioDb.Id, usuarioDb.UserName, usuarioDb.Role);
 
            httpContextAccessor.HttpContext!.Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,                  // JS no puede leerla -> protege contra robo por XSS
                Secure = true,                     // solo se envía por HTTPS
                SameSite = SameSiteMode.Strict,    // protege contra CSRF básico
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });
 
            return new IniciarSesionResponseDto(
                usuarioDb.Id,
                usuarioDb.UserName,
                usuarioDb.PrimerNombre,
                usuarioDb.Role
            );
        },
        ex => ex switch
        {
            UnauthorizedAccessException => new Error("Auth.InvalidCredentials", ex.Message),
            _ => new Error("Auth.Unexpected", ex.Message)
        });
    }
}