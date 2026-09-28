namespace ERP.TRAN.CrossLayers.API.Reportes.Requests;

public class ReporteProductosMasVendidosRequest
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? StoreId { get; set; }
    public int? WarehouseId { get; set; }
    public int Top { get; set; } = 10;
    public bool OrdenarPorTotal { get; set; } = false;
}
