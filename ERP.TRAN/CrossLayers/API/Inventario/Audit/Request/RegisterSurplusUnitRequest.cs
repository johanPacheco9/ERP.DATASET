namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;

/// <summary>
/// Registra una unidad sobrante: encontrada físicamente pero no registrada en BD.
/// </summary>
public class RegisterSurplusUnitRequest
{
    public int AuditId { get; set; }

    /// <summary>Serial físico del producto sobrante.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Producto al que se cree que pertenece (opcional).</summary>
    public int ProductId { get; set; }
    public int? ProductoVariantId { get; set; }

    /// <summary>Bodega física donde se encontró.</summary>
    public int PhysicalWarehouseId { get; set; }

    public string? Observations { get; set; }

    public string _AuditorAuth0Id { get; set; } = string.Empty;
}