using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

/// <summary>
/// Unidad física individual rastreada por Serial/IMEI.
/// </summary>
public class UnidadProducto : EntityWithtraceability
{
    public int Id { get; set; }
    public int ProductoVarianteId { get; set; }
    public int BodegaId { get; set; }

    // === TRAZABILIDAD FÍSICA ===
    public string SerialNumber { get; set; } = null!;  // IMEI o Serial de fábrica
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // === ESTADO Y UBICACIÓN ===
    public UnidadProductoStatus Status { get; set; } = UnidadProductoStatus.Available; // Available, Sold, Damaged, etc.
    public string? UbicacionFisica { get; set; }       // Ej: Estante A-3, Pasillo 2

    // === RELACIONES ===
    public ProductoVariante ProductoVariante { get; set; } = null!;
    public Warehouse Bodega { get; set; } = null!;
}