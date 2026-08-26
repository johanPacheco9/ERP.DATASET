using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

public class PosShift : EntityWithtraceability
{
    public int PosTerminalId { get; set; }
    public string CashierId { get; set; } = null!;
    public string CashierName { get; set; } = null!;
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    
    public decimal InitialCash { get; set; }
    public decimal CashSales { get; set; }
    public decimal CardSales { get; set; }
    public decimal TransferSales { get; set; }
    public decimal CreditSales { get; set; }
    
    public decimal CashWithdrawals { get; set; }
    public decimal CashAdditions { get; set; }
    
    public decimal TotalExpectedCash { get; set; }
    public decimal? ActualCash { get; set; }
    public decimal? Difference { get; set; }
    
    public PosShiftStatus Status { get; set; } = PosShiftStatus.Open;
    public string? Notes { get; set; }

    public PosTerminal PosTerminal { get; set; } = null!;
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
