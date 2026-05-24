namespace ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;


public record GetPaymentsBySaleResponseDto(
    int Id,
    decimal Amount,
    string Method,
    DateTime PaidAt,
    string? Reference,
    string? Notes,
    string CreatedBy
);