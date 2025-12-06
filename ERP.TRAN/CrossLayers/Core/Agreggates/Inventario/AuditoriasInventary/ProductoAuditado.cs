namespace ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.AuditoriasInventary;

public class ProductoAuditado
{
    public int Id { get; set; }

    public int AuditoriaId { get; set; }

    public ProductoAuditado Status { get; set; } = null!;
}
