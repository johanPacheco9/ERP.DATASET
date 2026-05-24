using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

public class SaleLineItem : EntityWithtraceability
{
    public int SaleId { get; set; }
    public int LineaProductoId { get; set; }
    public int? ProductoId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int? MovementId { get; set; }

    public Sale Sale { get; set; } = null!;
    public LineaProducto LineaProducto { get; set; } = null!;
    public Producto? Producto { get; set; }
}
