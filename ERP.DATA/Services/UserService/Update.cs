using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Users.Requests;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Services.UserService;
public partial class UserManager
{
    public async Task<Result<UserDetailDto>> Update(UpdateUserRequest request)
    {
        var user = await context.Usuarios.FirstOrDefaultAsync(s => s.Id == request.Id);
        if (user is null)
        {
            return Result<UserDetailDto>.Failure(new Error("User.NotFound", "User not found"));
        }

        return await Result<UserDetailDto>.TryAsync(async () =>
            {
                user.Email = request.Email;
                user.IsActive = request.IsActive;

                // El Role solo se toca si el request lo trae (la UI únicamente lo envía cuando el usuario que edita es Admin)
                if (request.Role.HasValue)
                {
                    user.Role = request.Role.Value;
                }

                // La contraseña solo se actualiza si se envió una nueva
                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    var hasher = new PasswordHasher<object>();
                    user.PasswordHash = hasher.HashPassword(null!, request.Password);
                }

                await context.SaveChangesAsync();

                return new UserDetailDto(
                    user.Id,
                    user.Email,
                    user.PrimerNombre,
                    user.SegundoNombre,
                    user.PrimerAPellido,
                    user.SegundoAPellido,
                    user.IsActive,
                    user.Role
                );
            },
            ex => new Error("User.UpdateFailed", ex.Message));
    }
}