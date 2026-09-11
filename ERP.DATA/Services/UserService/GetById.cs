using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Users.Enums;
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
            return Result<UserDetailDto>.Failure(Error.Failure("NotFound", "Usuario no encontrado."));
        }

        int userStoreId = 0;

        // Si es cajero u operación fija, usa el StoreId directo de su tabla
        if (usuario.Role == UserRole.Cashier)
        {
            userStoreId = usuario.StoreId ?? 0;
        }
        else 
        {
            // Si es Admin o Supervisor, busca su tienda por defecto o la primera en su lista multi-tienda
            userStoreId = await context.UsuarioStores
                .Where(st => st.UsuarioId == usuario.Id)
                .OrderByDescending(st => st.IsDefault) // Prioriza la marcada como principal
                .Select(st => st.StoreId)
                .FirstOrDefaultAsync();
        }

        var dto = new UserDetailDto(
            Id: usuario.Id,
            Email: usuario.Email,
            PrimerNombre: usuario.PrimerNombre,
            SegundoNombre: usuario.SegundoNombre,
            PrimerApellido: usuario.PrimerAPellido,
            SegundoApellido: usuario.SegundoAPellido,
            IsActive: usuario.IsActive,
            Role: usuario.Role,
            StoreId: userStoreId
        );

        return Result<UserDetailDto>.Success(dto);
    }
}