using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;

public sealed class UpdateUnitProductAuditRequest(string updaterAuth0Id) : BaseUpdateRequest(updaterAuth0Id)
{
    public int Id { get; set; }

    public UnitProductAuditStatus? Status { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoDiferencia { get; set; }
    public string? UbicacionFisica { get; set; }
    public string? EstadoFisico { get; set; }
    public bool? RequiereAccionCorrectiva { get; set; }
    
    public int AuditId { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (Id <= 0)
        {
            errors = "El Id debe ser mayor a cero.";
            return false;
        }
        return true;
    }
}
