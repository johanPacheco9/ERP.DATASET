using ERP.TRAN.CrossLayers.API.Users.Enums;

namespace ERP.TRAN.CrossLayers.API.Users.Responses;

public sealed record CrearUsuarioResponseDto(
    int Id,
    string UserName,
    string Email,
    UserRole Role
);