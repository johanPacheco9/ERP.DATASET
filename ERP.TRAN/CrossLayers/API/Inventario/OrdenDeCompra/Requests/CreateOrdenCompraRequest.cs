namespace ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Requests;

public class CreateOrdenCompraRequest
{
    public int ProveedorId { get; set; }
    public List<string>? Observaciones { get; set; }
    
    public List<CreateDetalleOrdenCompraRequest> Detalles { get; set; } = new();
}

public class CreateDetalleOrdenCompraRequest
{
    public int ProductoVarianteId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Descuento { get; set; } = 0m;
    public decimal Impuesto { get; set; } = 0m;
}