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
        var yaExiste = await context.Usuarios
            .AnyAsync(u => u.Email == request.Email);
        
        if (yaExiste)
        {
            return Result<CrearUsuarioResponseDto>.Failure(
                new Error("Usuarios.CorreoDuplicado", "Ya existe un usuario registrado con este correo."));
        }

        return await Result<CrearUsuarioResponseDto>.TryAsync(async () =>
            {
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
                
                var hasher = new PasswordHasher<object>();
                usuario.PasswordHash = hasher.HashPassword(null!, request.Password);
                context.Usuarios.Add(usuario);
                await context.SaveChangesAsync();

                if (request.StoreId is not null)
                {
                    var usuarioStore = new UsuarioStore
                    {
                        UsuarioId = usuario.Id,
                        StoreId = request.StoreId.Value
                    };
                    context.UsuarioStores.Add(usuarioStore);
                    await context.SaveChangesAsync();
                }

                return new CrearUsuarioResponseDto(
                    usuario.Id,
                    usuario.UserName,
                    usuario.Email,
                    usuario.Role
                );
            },
            ex => new Error("Usuarios.CreacionFallida", ex.Message));
    }
}