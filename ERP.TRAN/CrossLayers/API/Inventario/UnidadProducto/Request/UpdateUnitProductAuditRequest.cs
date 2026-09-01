using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Request;

public sealed class UpdateUnitProductAuditRequest() 
{
    public int Id { get; set; }

    public UnitProductAuditStatus? Status { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoDiferencia { get; set; }
    public string? UbicacionFisica { get; set; }
    public string? EstadoFisico { get; set; }
    public bool? RequiereAccionCorrectiva { get; set; }
    
    public int AuditId { get; set; }
    
    public int _UpdaterAuth0Id {get; set;}
}
