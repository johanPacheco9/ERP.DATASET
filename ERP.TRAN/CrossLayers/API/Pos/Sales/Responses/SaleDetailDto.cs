using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using System;
using System.Collections.Generic;

namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;

public sealed record SaleDetailDto(
    int Id,
    string SaleNumber,
    DateTime CreatedAt,
    string ClientName,
    string ClientIdentification,
    string? ClientEmail,
    string? ClientPhone,
    string? ClientAddress,
    string WarehouseName,
    decimal Subtotal,
    decimal TaxAmount,
    decimal Total,
    string StatusDisplay,
    PaymentStatus PaymentStatus,
    string? Notes,
    string? FactusInvoiceNumber,
    string? FactusStatus,
    string? FactusCufe,
    string? FactusQrUrl,
    IReadOnlyList<SaleLineDetailDto> Lines
);

public record SaleLineDetailDto(
    int Id,
    int ProductoVarianteId,
    string ProductName,
    string Sku,
    string? SerialNumber, // <-- Vital para la garantía
    int Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    decimal TaxAmount,
    decimal LineTotal,
    int? MovementId
);