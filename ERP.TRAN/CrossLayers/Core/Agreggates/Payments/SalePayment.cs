using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Payments;

public class SalePayment : EntityWithtraceability
{
    public int SaleId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string? Reference { get; set; } // número voucher, transacción, etc.
    public string? Notes { get; set; }

    // Navegación
    public Sale Sale { get; set; } = null!;
}