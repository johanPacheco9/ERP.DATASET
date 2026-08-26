namespace ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;

public record StockAlertDto(
    int WarehouseId,
    string WarehouseName,
    int ProductoVarianteId,
    int ProductoBaseId,
    string ProductCode,
    string ProductName,
    int CurrentStock,
    int ReservedStock,
    int MinimumStock,
    int MaximumStock,
    int ReorderQuantity
);