using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;

namespace ERP.TRAN.CrossLayers.API.Pos.Shifts.Responses;

public sealed record PosShiftDetailDto(
    int Id,
    int PosTerminalId,
    string TerminalName,
    string TerminalCode,
    string CashierId,
    string CashierName,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal InitialCash,
    decimal CashSales,
    decimal CardSales,
    decimal TransferSales,
    decimal CreditSales,
    decimal TotalSales,
    decimal CashWithdrawals,
    decimal CashAdditions,
    decimal TotalExpectedCash,
    decimal? ActualCash,
    decimal? Difference,
    PosShiftStatus Status,
    string? Notes,
    IReadOnlyList<SaleSummaryDto> Sales
);
