namespace ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;

/// <summary>
/// Información resumida del turno de caja activo que actualmente tiene abierto un cajero.
/// </summary>
public sealed record ActiveUserShiftDto(
    int ShiftId,
    int PosTerminalId,
    string TerminalName,
    string TerminalCode,
    int StoreId,
    string StoreName,
    int WarehouseId,
    string WarehouseName,
    DateTime OpenedAt,
    decimal InitialCash
);
