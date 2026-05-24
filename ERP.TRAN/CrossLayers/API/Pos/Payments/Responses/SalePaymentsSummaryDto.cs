namespace ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;

public record SalePaymentsSummaryDto(
    int SaleId,
    decimal SaleTotal,
    decimal TotalPaid,
    decimal Balance,
    string PaymentStatus,
    List<GetPaymentsBySaleResponseDto> Payments
);