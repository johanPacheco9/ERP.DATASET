using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
namespace ERP.DATASET.Components.Pages.Inventario.Auditorias;

public class UpdateUnitAuditedProductForm
{
    public int Id { get; set; }
    public int AuditId { get; set; }

    [Required(ErrorMessage = "El estado del conteo es obligatorio")]
    public UnitProductAuditStatus? Status { get; set; }
    
    public string? Observaciones { get; set; }
    public string? MotivoDiferencia { get; set; }
    public string? UbicacionFisica { get; set; }
    public string? EstadoFisico { get; set; }
    public bool RequiereAccionCorrectiva { get; set; }
}