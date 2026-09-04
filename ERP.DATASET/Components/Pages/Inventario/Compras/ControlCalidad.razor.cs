using ERP.TRAN.CrossLayers.API.Inventario.ControlCalidad.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Compras;

public partial class ControlCalidad
{
    [Parameter] public int Id { get; set; }
    private OrdenCompraDetailDto? Orden;
    private RecepcionDetailDto? Recepcion;
    private readonly List<LineaCalidadForm> Lineas = [];
    private string? Observaciones, Error;
    private bool Cargando = true, Guardando, YaExiste;

    protected override async Task OnInitializedAsync()
    {
        Orden = await Ordenes.GetById(Id, default);
        if (Orden?.RecepcionId is int recepcionId) Recepcion = await Recepciones.GetById(recepcionId, default);
        YaExiste = Orden?.QualityReviewId is not null;
        if (Orden is not null && Recepcion is not null && !YaExiste)
        {
            Lineas.AddRange(Recepcion.Detalles.Select(r =>
            {
                var ordenDetalle = Orden.Detalles.FirstOrDefault(d => d.Id == r.Id || d.ProductoVarianteId == r.ProductoVarianteId);
                return new LineaCalidadForm(ordenDetalle?.Id ?? 0, r.ProductoVarianteId, r.ProductoNombre ?? ordenDetalle?.ProductoNombre ?? "Producto", r.SKU ?? ordenDetalle?.SKU ?? "", r.CantidadRecibida, r.CantidadRecibida);
            }));
        }
        Cargando = false;
    }

    private async Task Guardar()
    {
        Error = null;
        if (Recepcion is null || Lineas.Count == 0 || Lineas.Any(l => l.DetalleOrdenCompraId <= 0 || l.Aprobada < 0 || l.Rechazada < 0 || l.Aprobada + l.Rechazada != l.Recibida || (l.Rechazada > 0 && string.IsNullOrWhiteSpace(l.Motivo))))
        { Error = "Cada línea debe distribuir exactamente lo recibido entre aprobado y rechazado; explica cualquier rechazo."; return; }
        Guardando = true;
        var request = new CreateQualityReviewRequest { OrdenCompraId = Id, RecepcionId = Recepcion.Id, ObservacionesGenerales = Observaciones, Items = Lineas.Select(l => new QualityReviewItemRequest { DetalleOrdenCompraId = l.DetalleOrdenCompraId, ProductoVarianteId = l.ProductoVarianteId, CantidadRecibida = l.Recibida, CantidadAprobada = l.Aprobada, CantidadRechazada = l.Rechazada, MotivoRechazo = l.Motivo }).ToList() };
        var result = await Calidad.Create(request, default);
        Guardando = false;
        if (result.IsSuccess) Navigation.NavigateTo($"/inventario/compras/{Id}"); else Error = result.Error.Message;
    }

    private sealed class LineaCalidadForm(int detalleOrdenCompraId, int productoVarianteId, string nombre, string sku, decimal recibida, decimal aprobada) { public int DetalleOrdenCompraId { get; } = detalleOrdenCompraId; public int ProductoVarianteId { get; } = productoVarianteId; public string Nombre { get; } = nombre; public string Sku { get; } = sku; public decimal Recibida { get; } = recibida; public decimal Aprobada { get; set; } = aprobada; public decimal Rechazada { get; set; } public string? Motivo { get; set; } }
}
