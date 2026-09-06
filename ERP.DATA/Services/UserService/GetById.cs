using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.UserService;

public partial class UserManager
{
    public async Task<Result<UserDetailDto>> GetById(int id)
    {
        var usuario = await context.Usuarios.FirstOrDefaultAsync(s => s.Id == id);
        if (usuario == null)
        {
            return Result<UserDetailDto>.Failure(Error.Failure("NotFound","Usuario no encontrado."));
        }

        var dto = new UserDetailDto(
            Id: usuario.Id,
            Email: usuario.Email,
            PrimerNombre: usuario.PrimerNombre,
            SegundoNombre: usuario.SegundoNombre,
            PrimerApellido: usuario.PrimerAPellido,
            SegundoApellido: usuario.SegundoAPellido,
            IsActive: usuario.IsActive,
            Role: usuario.Role
        );

        return Result<UserDetailDto>.Success(dto);
    }
}