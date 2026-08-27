using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Pos.Shifts.Requests;

public sealed class OpenShiftRequest : BaseCreateRequest
{
    public int PosTerminalId { get; set; }
    public int CashierId { get; set; }
    public string CashierName { get; set; } = "Cajero de Turno";
    public decimal InitialCash { get; set; } // Base de efectivo
    public string? Notes { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (PosTerminalId <= 0)
            list.Add("Seleccione una caja para abrir el turno.");
        if (InitialCash < 0)
            list.Add("La base de caja no puede ser negativa.");
        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}
