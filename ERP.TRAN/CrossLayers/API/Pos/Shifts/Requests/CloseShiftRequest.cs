using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Pos.Shifts.Requests;

public sealed class CloseShiftRequest : BaseCreateRequest
{
    public int PosShiftId { get; set; }
    public decimal ActualCash { get; set; } // Dinero real contado en gaveta
    public string? Notes { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (PosShiftId <= 0)
            list.Add("Identificador de turno inválido.");
        if (ActualCash < 0)
            list.Add("El monto contado no puede ser negativo.");
        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}
