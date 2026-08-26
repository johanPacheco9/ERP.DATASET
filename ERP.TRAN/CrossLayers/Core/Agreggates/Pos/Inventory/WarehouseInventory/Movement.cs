using System.ComponentModel.DataAnnotations.Schema;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

/// <summary>
/// Movimiento de Kárdex. Registra entradas, salidas y ajustes de stock por variante.
/// </summary>
public class Movement : EntityWithtraceability
{
    public int Id { get; set; }

    // === UBICACIÓN Y SKU ===
    public int WarehouseId { get; set; }
  

    // === CLASIFICACIÓN Y CANTIDAD ===
    public TipoMovimiento Type { get; set; }
    public int Quantity { get; set; }

    // === VALORACIÓN DE INVENTARIO (FIFO / LIFO / Promedio Ponderado) ===
    [Column(TypeName = "decimal(15,4)")]
    public decimal UnitCost { get; set; }

    // Calculado en memoria
    [NotMapped]
    public decimal TotalCost => Quantity * UnitCost;

    // === TRAZABILIDAD Y REFERENCIAS CRUZADAS ===
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; } // 'compra', 'venta', 'ajuste_manual', 'traslado', etc.

    // === LOTES Y VENCIMIENTOS (Aplica si la variante o base maneja lotes) ===
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // === JUSTIFICACIÓN ===
    public string? Motive { get; set; }
    public string? Observations { get; set; }

    // === NAVEGACIÓN ===
    public Warehouse Warehouse { get; set; } = null!;
    
    /// <summary>
    /// Relacion con Unidad producto si es para un producto con serial.
    /// </summary>
    public int? UnidadProductoId { get; set; }
    public UnidadProducto? UnidadProducto { get; set; }
    
    /// <summary>
    /// Relacion con la variante.
    /// </summary>
    public int ProductoVarianteId { get; set; }
    public ProductoVariante ProductoVariante { get; set; } = null!;
}