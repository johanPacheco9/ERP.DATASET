using System.ComponentModel.DataAnnotations.Schema;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

/// <summary>
/// Movimiento de Kárdex (Cabecera). Registra la operación global de entradas, salidas y transferencias.
/// </summary>
public class Movement : EntityWithtraceability
{
    public int Id { get; set; }
    
    // === CLASIFICACIÓN Y CANTIDAD ===
    public TipoMovimiento Type { get; set; }
    public int Quantity { get; set; }

    // === VALORACIÓN DE INVENTARIO (FIFO / LIFO / Promedio Ponderado) ===
    [Column(TypeName = "decimal(15,4)")]
    public decimal UnitCost { get; set; }

    // Calculado en memoria
    [NotMapped]
    public decimal TotalCost => Quantity * UnitCost;
    
    // === LOTES Y VENCIMIENTOS (Aplica si la variante o base maneja lotes) ===
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // === JUSTIFICACIÓN ===
    public string? Motive { get; set; }
    public string? Observations { get; set; }
    
    // === UBICACIÓN ===
    /// <summary>
    /// Bodega principal afectada (Entrada, Salida, Baja) o Bodega de Origen en una Transferencia.
    /// </summary>
    public int OrigenWarehouseId { get; set; }
    public Warehouse OrigenWarehouse { get; set; } = null!;

    /// <summary>
    /// Bodega de destino (Únicamente aplica para transferencias entre bodegas).
    /// </summary>
    public int? DestinationWarehouseId { get; set; }
    public Warehouse? DestinationWarehouse { get; set; }
    
    public int? AuditId { get; set; }
    public Audit? Audit { get; set; }
    
    public int? SaleId { get; set; }
    public Sale? Sale { get; set; }
    
    public int? CompraId { get; set; }
    public OrdenCompra? OrdenDeCompra { get; set; }
    
    // === DETALLE (RELACIÓN 1 A MUCHOS) ===
    /// <summary>
    /// Detalle de los productos unitarios o variantes afectadas en este movimiento global.
    /// </summary>
    public ICollection<UnitProductMovement> UnitProductMovements { get; set; } = new List<UnitProductMovement>();
}