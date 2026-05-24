using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;

public sealed record SaleDetailDto(
    int Id,
    string SaleNumber,
    DateTime CreatedAt,
    string ClientName,
    string ClientIdentification,
    string WarehouseName,
    decimal Subtotal,
    decimal Total,
    string StatusDisplay,
    PaymentStatus PaymentStatus,
    string? Notes,
    IReadOnlyList<SaleLineDetailDto> Lines
);

public sealed record SaleLineDetailDto(
    int Id,
    string ProductName,
    string? SerialOrSku,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal,
    int? MovementId
);
