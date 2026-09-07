namespace ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Requests;

public sealed class AproveOrdenCompraRequest
{
    public int OrdenCompraId { get; set; }

    //  faltaba esta propiedad, Actions.cs (OrdenCompraController) ya la usaba
    // (request.Observaciones) pero la clase nunca la declaroooooooooo johanHagaBienlasCosas.
    public string? Observaciones { get; set; }
}