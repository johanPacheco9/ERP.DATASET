using ERP.TRAN.CrossLayers.API.Users.Enums;

namespace ERP.TRAN.CrossLayers.API.Users.Responses;

public sealed record IniciarSesionResponseDto(
    int UserId,
    string Usuario,
    string NombreCompleto,
    UserRole Rol
);