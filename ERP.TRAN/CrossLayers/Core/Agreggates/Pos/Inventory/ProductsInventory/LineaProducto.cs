using ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

public class LineaProducto : EntityWithtraceability
{
    // === IDENTIDAD ===
    public string Code { get; set; } = null!;     // Código base, prefijo de los SKUs
    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }

    // === PRECIOS BASE (referencial, Producto puede sobreescribir) ===
    public decimal CostoUnitario { get; set; } = 0m;
    public decimal PrecioVenta { get; set; } = 0m;

    // === FISCAL ===
    public decimal PorcentajeIVA { get; set; } = 0.19m;
    public decimal PorcentajeICA { get; set; }
    public decimal ImpuestoEspecifico { get; set; }
    public decimal ArancelImportacion { get; set; }
    public bool ExentoIVA { get; set; }
    public bool GravadoICA { get; set; }
    public string? CodigoTributario { get; set; }   // Partida arancelaria DIAN

    // === FÍSICOS ===
    public decimal Peso { get; set; } = 0m;
    public decimal Volumen { get; set; } = 0m;
    public string? Dimensiones { get; set; }
    public bool EsPerecedero { get; set; }
    public string UnidadMedida { get; set; } = "unidades";

    // === CATEGORIZACIÓN ===
    public int CategoryId { get; set; }
    public int? SupplierId { get; set; }

    // === METADATOS ===
    public string? ImagenUrl { get; set; }
    public string? Notas { get; set; }
    public string? Tags { get; set; }
    public LineaProductoStatus Status { get; set; }

    // === RELACIONES ===
    public Category Categoria { get; set; } = null!;
    
    public Supplier? Proveedor { get; set; }
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    
    public ICollection<WarehouseStock> Stock{ get; set; } = new List<WarehouseStock>();
    
    // === HELPERS ===
    public bool TieneVariantes => Productos.Count > 1;
    
    public bool RequiereSerial {get; set; }
}