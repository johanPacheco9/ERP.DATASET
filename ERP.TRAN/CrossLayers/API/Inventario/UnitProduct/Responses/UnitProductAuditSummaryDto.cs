namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;

public class UnitProductAuditSummaryDto
{
    public int Id { get; set; }
    public string Serial { get; set; } = null!;
    public string? ProductoName { get; set; }
    public string? BodegaName { get; set; }
    public string StatusDisplay { get; set; } = null!;
    
    // ATRIBUTOS EXPUESTOS ADICIONALES PARA LA REACCION DE LA GRILLA
    public string? UbicacionFisica { get; set; }
    public string? EstadoFisico { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoDiferencia { get; set; }
    public bool RequiereAccionCorrectiva { get; set; }
}