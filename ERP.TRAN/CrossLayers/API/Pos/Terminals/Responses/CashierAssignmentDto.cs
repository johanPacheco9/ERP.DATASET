using ERP.TRAN.CrossLayers.API.Users.Enums;

namespace ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;

/// <summary>
/// Representa a un cajero y las sucursales asignadas en las que tiene autorización para operar cajas POS.
/// </summary>
public sealed record CashierAssignmentDto(
    int UsuarioId,
    string NombreCompleto,
    string UserName,
    string Email,
    UserRole Role,
    bool IsActive,
    List<StoreAssignmentSummaryDto> AssignedStores
);

public sealed record StoreAssignmentSummaryDto(
    int StoreId,
    string StoreName,
    bool IsDefault
);
