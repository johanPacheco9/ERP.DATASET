using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Requests;

public sealed class CreateOrdenCompraRequest : BaseCreateRequest
{
    public int ProveedorId { get; set; }
    public List<string>? Observaciones { get; set; }

    public List<CreateDetalleOrdenCompraRequest> Detalles { get; set; } = new();

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (ProveedorId <= 0)
            list.Add("Seleccione un proveedor.");
        if (Detalles == null || !Detalles.Any())
            list.Add("Agregue al menos un producto a la orden de compra.");
        else
        {
            foreach (var (detalle, i) in Detalles.Select((d, idx) => (d, idx + 1)))
            {
                if (detalle.ProductoVarianteId <= 0)
                    list.Add($"Detalle {i}: variante de producto inválida.");
                if (detalle.Cantidad <= 0)
                    list.Add($"Detalle {i}: la cantidad debe ser mayor a cero.");
                if (detalle.CostoUnitario < 0)
                    list.Add($"Detalle {i}: el costo unitario no puede ser negativo.");
            }
        }

        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}

public class CreateDetalleOrdenCompraRequest
{
    public int ProductoVarianteId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Descuento { get; set; } = 0m;
    public decimal Impuesto { get; set; } = 0m;
}