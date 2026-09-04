namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras.Recepciones;

public class DetalleRecepcionCompra
{
    public int Id { get; set; }

    public int RecepcionCompraId { get; set; }
    public RecepcionCompra RecepcionCompra { get; set; } = null!;

    public int DetalleOrdenCompraId { get; set; }
    public DetalleOrdenCompra DetalleOrdenCompra { get; set; } = null!;

    public int ProductoVarianteId { get; set; }

    public decimal CantidadEsperada { get; set; }
    public decimal CantidadRecibida { get; set; } 
    
    public string? ObservacionItem { get; set; }
}