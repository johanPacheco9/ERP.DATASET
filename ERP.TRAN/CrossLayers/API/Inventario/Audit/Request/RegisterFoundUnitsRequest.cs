namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
public class RegisterFoundUnitsRequest
{
    /// <summary>ID de la auditoría en curso.</summary>
    public int AuditId { get; set; }

    /// <summary>
    /// Lista de seriales encontrados.
    /// Un solo elemento = escaneo individual. Varios = bulk.
    /// </summary>
    public List<string> ProductsIds { get; set; } = new();

    /// <summary>
    /// Bodega física donde se encontró la unidad.
    /// Si difiere de la bodega en BD se marca como LocationMismatch.
    /// </summary>
    public int PhysicalWarehouseId { get; set; }

    /// <summary>Auth0 ID del auditor que realiza el conteo.</summary>
    public string _AuditorAuth0Id { get; set; } = string.Empty;
}