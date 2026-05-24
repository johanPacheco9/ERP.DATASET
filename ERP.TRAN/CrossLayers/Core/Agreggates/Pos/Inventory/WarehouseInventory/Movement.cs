using System.ComponentModel.DataAnnotations.Schema;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

public class Movement : EntityWithtraceability
{
    public int LineaProductoId { get; set; }

    public int? ProductId { get; set; }
    public int WarehouseId { get; set; }
    public TipoMovimiento Type { get; set; }

    public int Quantity { get; set; }

    // Para FIFO/LIFO
    public decimal UnitCost { get; set; } // DECIMAL(15,4)

    // Calculado en memoria
    public decimal TotalCost => Quantity * UnitCost;

    // Referencias a otros módulos (compra, venta, etc.)
    public int? ReferenceId { get; set; }
    public string? ReferenceTye { get; set; } // 'compra','venta','ajuste_manual', etc.

    // Lote / vencimiento
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // Motive y observaciones
    public string? Motive { get; set; }
    public string? Observations { get; set; }

    public LineaProducto LineaProducto { get; set; } = null!;
    
    public Producto? Producto { get; set; }  // ← agrega el ?
    public Warehouse Warehouse { get; set; } = null!;
}
