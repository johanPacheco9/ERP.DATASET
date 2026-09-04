using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Compras;

public partial class RegistrarRecepcion
{
    [Parameter] public int Id { get; set; }
    private OrdenCompraDetailDto? Orden;
    private List<ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses.WarehouseSummaryDto> ListaBodegas = [];
    private List<LineaRecepcionForm> Lineas = [];
    private int BodegaId;
    private string? Guia, Observaciones, Error;
    private bool Cargando = true, Guardando;

    protected override async Task OnInitializedAsync()
    {
        Orden = await Ordenes.GetById(Id, default);
        ListaBodegas = (await Bodegas.List(new ListWarehousesRequest { PageSize = -1 }, default)).ToList();
        if (Orden is not null) Lineas = Orden.Detalles.Select(d => new LineaRecepcionForm(d.Id, d.ProductoVarianteId, d.ProductoNombre ?? "Producto", d.SKU ?? "", d.Cantidad, d.Cantidad)).ToList();
        Cargando = false;
    }

    private async Task Guardar()
    {
        Error = null;
        if (BodegaId <= 0 || !Lineas.Any(l => l.Recibida > 0) || Lineas.Any(l => l.Recibida < 0 || l.Recibida > l.Esperada)) { Error = "Selecciona una bodega y registra cantidades válidas."; return; }
        Guardando = true;
        var request = new CreateRecepcionRequest { OrdenCompraId = Id, BodegaId = BodegaId, GuiaRemisionProveedor = Guia, Observaciones = Observaciones, Detalles = Lineas.Where(l => l.Recibida > 0).Select(l => new DetalleRecepcionRequest { DetalleOrdenCompraId = l.DetalleId, ProductoVarianteId = l.ProductoVarianteId, CantidadEsperada = l.Esperada, CantidadRecibida = l.Recibida, ObservacionItem = l.Observacion }).ToList() };
        var result = await Recepciones.Crear(request, 1, default);
        Guardando = false;
        if (result.IsSuccess) Navigation.NavigateTo($"/inventario/compras/{Id}/calidad"); else Error = result.Error.Message;
    }

    private sealed class LineaRecepcionForm(int detalleId, int productoVarianteId, string nombre, string sku, decimal esperada, decimal recibida) { public int DetalleId { get; } = detalleId; public int ProductoVarianteId { get; } = productoVarianteId; public string Nombre { get; } = nombre; public string Sku { get; } = sku; public decimal Esperada { get; } = esperada; public decimal Recibida { get; set; } = recibida; public string? Observacion { get; set; } }
}
