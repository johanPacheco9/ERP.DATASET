using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

public class SaleLineItem : EntityWithtraceability
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    
    public int ProductoVarianteId { get; set; }
    
    // === NUEVO: Para saber el ítem físico exacto si está serializado ===
    public int? UnidadProductoId { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public decimal TaxRate { get; set; } = 0.19m;
    public decimal TaxAmount { get; set; } = 0m;
    public decimal LineTotal { get; set; }
    public int? MovementId { get; set; }

    // === NAVEGACIÓN ===
    public Sale Sale { get; set; } = null!;
    public ProductoVariante ProductoVariante { get; set; } = null!;
    public UnidadProducto? UnidadProducto { get; set; } // Opcional, si tienes la entidad creada
}
