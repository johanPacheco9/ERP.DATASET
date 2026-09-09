using ERP.TRAN.CrossLayers.API.Users.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Users.Requests;

public sealed class UpdateUserRequest(int updaterId) : BaseUpdateRequest(updaterId)
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public UserRole? Role { get; set; }
    public bool IsActive { get; set; }
    public string? Password { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        if (Id <= 0)
        {
            errors = "El ID del usuario no es válido.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            errors = "El correo electrónico no puede estar vacío.";
            return false;
        }

        errors = null;
        return true;
    }
}