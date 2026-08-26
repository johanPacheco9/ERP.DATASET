namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;

public sealed record SaleSummaryDto(
    int Id,
    string SaleNumber,
    DateTime CreatedAt,
    string ClientName,
    string WarehouseName,
    decimal Subtotal,
    decimal TaxAmount,
    decimal Total,
    string StatusDisplay,
    string PaymentStatusDisplay,
    int LineCount,
    string? FactusStatus = null,
    string? FactusInvoiceNumber = null
);
