namespace ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Requests;

public class CreateRecepcionRequest
{
    public int OrdenCompraId { get; set; }
    public int BodegaId { get; set; }
    public string? GuiaRemisionProveedor { get; set; }
    public string? Observaciones { get; set; }
    public List<DetalleRecepcionRequest> Detalles { get; set; } = new();
}

public class DetalleRecepcionRequest
{
    public int DetalleOrdenCompraId { get; set; }
    public int ProductoVarianteId { get; set; }
    public decimal CantidadEsperada { get; set; }
    public decimal CantidadRecibida { get; set; }
    public string? ObservacionItem { get; set; }
}
