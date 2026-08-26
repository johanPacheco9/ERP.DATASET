namespace ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;

public sealed record PosTerminalDto(
    int Id,
    string Name,
    string Code,
    int StoreId,
    string StoreName,
    int WarehouseId,
    string WarehouseName,
    string Prefix,
    long CurrentConsecutive,
    string? DianResolutionNumber,
    bool IsActive,
    bool HasActiveShift,
    int? ActiveShiftId,
    string? ActiveCashierName
);
