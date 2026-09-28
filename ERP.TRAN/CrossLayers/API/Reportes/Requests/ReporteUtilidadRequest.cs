namespace ERP.TRAN.CrossLayers.API.Reportes.Requests;

public class ReporteUtilidadRequest
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? StoreId { get; set; }
    public int? WarehouseId { get; set; }
}
