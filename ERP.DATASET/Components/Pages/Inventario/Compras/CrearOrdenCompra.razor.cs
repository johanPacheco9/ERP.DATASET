using ERP.DATA.Services.Inventario.ProveedorService;
using ERP.DATA.Services.InventarioService.ProductoBaseService;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;

namespace ERP.DATASET.Components.Pages.Inventario.Compras;

public partial class CrearOrdenCompra
{
    private readonly List<LineaOrdenForm> Lineas = [new()];
    private List<ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory.Proveedor> Proveedores = [];
    private List<ProductoOpcion> Productos = [];
    private int ProveedorId;
    private string? Observacion;
    private bool Cargando = true;
    private bool Guardando;
    private string? Error;

    protected override async Task OnInitializedAsync()
    {
        Proveedores = await ProveedoresService.ListAsync(1_000, default);
        var productos = await ProductosService.ListAsync(new ListProductRequest { PageSize = -1 }, null, null, null, default);
        Productos = productos.SelectMany(p => p.ProductoVariantes.Select(v => new ProductoOpcion(v.Id ?? 0, p.Nombre, v.CodigoVariante, v.CostoUnitario ?? p.CostoUnitario))).Where(p => p.Id > 0).ToList();
        Cargando = false;
    }

    private void AgregarLinea() => Lineas.Add(new());
    private void EliminarLinea(LineaOrdenForm linea) => Lineas.Remove(linea);

    private async Task Guardar()
    {
        Error = null;
        if (ProveedorId <= 0 || Lineas.Count == 0 || Lineas.Any(l => l.ProductoVarianteId <= 0 || l.Cantidad <= 0 || l.CostoUnitario < 0))
        { Error = "Selecciona un proveedor y completa al menos una línea válida."; return; }

        Guardando = true;
        var request = new CreateOrdenCompraRequest { ProveedorId = ProveedorId, Observaciones = string.IsNullOrWhiteSpace(Observacion) ? null : [Observacion], Detalles = Lineas.Select(l => new CreateDetalleOrdenCompraRequest { ProductoVarianteId = l.ProductoVarianteId, Cantidad = l.Cantidad, CostoUnitario = l.CostoUnitario, Descuento = l.Descuento, Impuesto = l.Impuesto }).ToList() };
        var result = await Ordenes.Create(request, default);
        Guardando = false;
        if (result.IsSuccess) Navigation.NavigateTo($"/inventario/compras/{result.Value.Id}"); else Error = result.Error.Message;
    }

    private sealed class LineaOrdenForm { public int ProductoVarianteId { get; set; } public decimal Cantidad { get; set; } = 1; public decimal CostoUnitario { get; set; } public decimal Descuento { get; set; } public decimal Impuesto { get; set; } }
    private sealed record ProductoOpcion(int Id, string Nombre, string Sku, decimal Costo);
}
