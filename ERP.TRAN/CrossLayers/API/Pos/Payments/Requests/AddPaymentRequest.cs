using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;

namespace ERP.TRAN.CrossLayers.API.Pos.Payments.Requests;

public class AddPaymentRequest
{
    public int SaleId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public int _CreatorId { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (SaleId <= 0)
            list.Add("La venta es obligatoria.");
        if (Amount <= 0)
            list.Add("El valor del abono debe ser mayor a cero.");

        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}
