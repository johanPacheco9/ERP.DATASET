using ERP.TRAN.CrossLayers.API.Users.Enums;
namespace ERP.TRAN.CrossLayers.API.Users.Responses;

public record UserDetailDto(
    int Id,
    string Email,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    bool IsActive,
    UserRole Role,
    int StoreId
);