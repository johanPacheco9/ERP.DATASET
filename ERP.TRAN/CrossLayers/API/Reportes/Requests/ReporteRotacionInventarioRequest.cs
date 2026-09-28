namespace ERP.TRAN.CrossLayers.API.Reportes.Requests;

public class ReporteRotacionInventarioRequest
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int? BranchId { get; set; }
    public int? WarehouseId { get; set; }
    public int? CategoriaId { get; set; }
    public string? Search { get; set; }
    public string? Clasificacion { get; set; } // "Alta", "Media", "Baja", "SinMovimiento", "Todos"
}
