namespace ERP.TRAN.CrossLayers.API.Reportes.Requests;

public class ReporteCajasMasVendedorasRequest
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int? StoreId { get; set; }
    public int? PosTerminalId { get; set; }
}
