using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;

namespace ERP.DATA.Services.InventarioService.AuditService.Responses;

public class AuditUnitDto
{
    public int Id { get; set; }
    public int UnidadProductoId { get; set; }
    public string Serial { get; set; } = string.Empty;
    public string ProductoName { get; set; } = string.Empty;
    public UnitProductAuditStatus StatusCode { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string UbicacionFisica { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
}