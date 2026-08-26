using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

public class LoteStock : EntityWithtraceability
{
    public int Id { get; set; }
    public int ProductoId { get; set; }        // SKU específico (ej: POLO-ROJ-M)
    public int BodegaId { get; set; }          // Dónde está guardado
    
    public string Lote { get; set; } = null!;   // Código del lote (ej: LOT-2026-001)
    public DateTime? FechaVencimiento { get; set; }
    
    public int CantidadInicial { get; set; }    // Ej: Entraron 60
    public int CantidadDisponible { get; set; } // Quedan 42
    public decimal CostoUnitarioCompra { get; set; }

    // Navegación
    public ProductoVariante ProductoVariante { get; set; } = null!;
    public Warehouse Bodega { get; set; } = null!;
}