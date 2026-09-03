namespace ERP.TRAN.CrossLayers.API.Stores.Responses;

public sealed record StoreSummaryDto(
    int Id,
    string Name,
    string? Description,
    bool IsMainStore,
    string Type,
    int WarehouseCount);