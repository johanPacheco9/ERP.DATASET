using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

/// <summary>
/// Variante comercial específica (SKU). Representa lo que se escanea y añade al carrito.
/// </summary>
public class ProductoVariante : EntityWithtraceability
{
    public int Id { get; set; }
    public int ProductoBaseId { get; set; }

    public ProductoVarianteStatus Status { get; set; }
    
    // === IDENTIFICACIÓN COMERCIAL ===
    public string SKU { get; set; } = null!;           // Ej: POLO-ROJ-M
    public string? CodigoBarras { get; set; }

    // === ATRIBUTOS DINÁMICOS ===
    public string? Atributos { get; set; }             // JSON: {"Color": "Rojo", "Talla": "M"}

    // === SOBREESCRITURA DE PRECIOS (null = Hereda de ProductoBase) ===
    public decimal? PrecioVenta { get; set; }
    public decimal? CostoUnitario { get; set; }

    // === RELACIONES ===
    public ProductoBase ProductoBase { get; set; } = null!;
    public ICollection<WarehouseStock> Stocks { get; set; } = new List<WarehouseStock>();
    public ICollection<Movement> Movimientos { get; set; } = new List<Movement>();
    public ICollection<UnidadProducto> UnidadesFisicas { get; set; } = new List<UnidadProducto>();
}