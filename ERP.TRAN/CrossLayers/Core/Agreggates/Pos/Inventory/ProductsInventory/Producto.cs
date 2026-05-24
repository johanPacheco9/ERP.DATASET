using System.ComponentModel;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

public class Producto :EntityWithtraceability
{
    public int LineaProductoId { get; set; }
    
    // Identificación
    public string SKU { get; set; } = null!;
    public string? CodigoBarras { get; set; }
    public string? Serial { get; set; }        // null si no requiere serial
    
    // Lote / vencimiento
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // Precios (null = heredar de LineaProducto)
    public decimal? PrecioVenta { get; set; }
    public decimal? CostoUnitario { get; set; }

    // Ubicación y estado
    public int BodegaId { get; set; }
    public ProductoStatus Status { get; set; }
    public string? Atributos { get; set; }

    // Relaciones
    public LineaProducto LineaProducto { get; set; } = null!;
    public Warehouse Bodega { get; set; } = null!;
    public ICollection<Movement> Movimientos { get; set; } = new List<Movement>();
}