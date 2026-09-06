using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.UserService;

public partial class UserManager
{
    public async Task<Result<CrearUsuarioResponseDto>> CrearUsuario(CrearUsuarioRequest request)
    {
        return await Result<CrearUsuarioResponseDto>.TryAsync(async () =>
            {
                var yaExiste = await context.Usuarios
                    .AnyAsync(u => u.Email == request.Email);

                if (yaExiste)
                {
                    throw new InvalidOperationException("Ya existe un usuario con ese correo electrónico");
                }

                var usuario = new Usuario
                {
                    PrimerNombre = request.PrimerNombre,
                    SegundoNombre = request.SegundoNombre,
                    PrimerAPellido = request.PrimerApellido,
                    SegundoAPellido = request.SegundoApellido,
                    Email = request.Email,
                    Role = request.Role,
                    IsActive = true
                };

                // Mismo PasswordHasher<object> que usa IniciarSesion -> hash 100% compatible.
                var hasher = new PasswordHasher<object>();
                usuario.PasswordHash = hasher.HashPassword(null!, request.Password);

                context.Usuarios.Add(usuario);
                await context.SaveChangesAsync();

                return new CrearUsuarioResponseDto(
                    usuario.Id, // AJUSTA: confirma que Id existe en EntityWithtraceability
                    usuario.UserName,
                    usuario.Email,
                    usuario.Role
                );
            },
            ex => ex switch
            {
                InvalidOperationException => new Error("Usuarios.CorreoDuplicado", ex.Message),
                _ => new Error("Usuarios.CreacionFallida", ex.Message)
            });
    }
}