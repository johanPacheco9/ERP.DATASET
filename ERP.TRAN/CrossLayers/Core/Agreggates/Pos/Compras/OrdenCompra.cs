using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;

public class OrdenCompra
{
    public int Id { get; set; }

    public int ProveedorId { get; set; }

    public DateTime Fecha { get; set; }

    public OrdenCompraStatus Status { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }

    public string? Observaciones { get; set; }

    public Proveedor Proveedor { get; set; } = null!;

    public ICollection<DetalleOrdenCompra> Detalles { get; set; }
        = new List<DetalleOrdenCompra>();
}