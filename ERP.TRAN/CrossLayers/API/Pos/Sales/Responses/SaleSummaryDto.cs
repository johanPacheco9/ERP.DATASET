namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;

public sealed record SaleSummaryDto(
    int Id,
    string SaleNumber,
    DateTime CreatedAt,
    string ClientName,
    string WarehouseName,
    decimal Total,
    string StatusDisplay,
    string PaymentStatusDisplay,
    int LineCount
);
