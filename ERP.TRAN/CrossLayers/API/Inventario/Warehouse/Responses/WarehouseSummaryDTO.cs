using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;

namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;

public sealed record WarehouseSummaryDto(
    int Id,
    string Code,
    string Name,
    string? Location,
    WarehouseType Type,
    int StoreId,
    string StoreName,
    int TotalProducts,
    decimal? MaxCapacity,
    bool IsActive
);