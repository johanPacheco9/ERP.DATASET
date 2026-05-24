namespace ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;

public record ReceivableSummaryDto(
    int SaleId,
    string SaleNumber,
    DateTime CreatedAt,
    string ClientName,
    string ClientIdentification,
    decimal SaleTotal,
    decimal TotalPaid,
    decimal Balance,
    string PaymentStatus
);
