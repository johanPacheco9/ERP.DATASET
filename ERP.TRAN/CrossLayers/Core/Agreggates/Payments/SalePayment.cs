using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Payments;

public class SalePayment : EntityWithtraceability
{
    public int SaleId { get; set; }
    public decimal Amount { get; set; }          // Lo que se aplica realmente al pago de la venta
    public PaymentMethod Method { get; set; }
    public decimal? CashReceived { get; set; }    // NUEVO: solo aplica si Method == Cash
    public decimal? ChangeGiven { get; set; }     // NUEVO: CashReceived - Amount, solo si Method == Cash
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string? Reference { get; set; }
    public string? Notes { get; set; }

    public Sale Sale { get; set; } = null!;
}