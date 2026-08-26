using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;

namespace ERP.TRAN.CrossLayers.API.Pos.Shifts.Responses;

public sealed record PosShiftSummaryDto(
    int Id,
    int PosTerminalId,
    string TerminalName,
    string TerminalCode,
    string CashierName,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal InitialCash,
    decimal CashSales,
    decimal CardSales,
    decimal TransferSales,
    decimal CreditSales,
    decimal TotalSales,
    decimal TotalExpectedCash,
    decimal? ActualCash,
    decimal? Difference,
    PosShiftStatus Status,
    int TotalTransactions
);
