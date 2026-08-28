namespace ERP.TRAN.CrossLayers.API.Pos.Stores.Responses;

public sealed record StoreSummaryDto(
    int Id,
    string Name,
    string? Description,
    bool IsMainStore,
    bool IsActive,
    int WarehousesCount,
    int TerminalsCount
);