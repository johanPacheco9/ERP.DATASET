using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;

public class DetalleOrdenCompra
{
    public int Id { get; set; }

    public int OrdenCompraId { get; set; }

    public int ProductoVarianteId { get; set; }

    //Cantidad de productos que ingresaran en la compra(genera movimiento de inventario tipo entrada => antes, a control de calidad) 
    public decimal Cantidad { get; set; }

    public decimal CostoUnitario { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public OrdenCompra OrdenCompra { get; set; } = null!;

    public ProductoVariante ProductoVariante { get; set; } = null!;
}