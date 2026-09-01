using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

/// <summary>
/// Catálogo general del producto base. Define precios referenciales y reglas fiscales.
/// </summary>
public class ProductoBase : EntityWithtraceability
{
    public int Id { get; set; }

    // === IDENTIDAD ===
    public string Code { get; set; } = null!;        // Prefijo base
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // === PRECIOS BASE (Referenciales) ===
    public decimal CostoUnitario { get; set; } = 0m;
    public decimal PrecioVenta { get; set; } = 0m;

    // === FISCAL (DIAN) ===
    public decimal PorcentajeIVA { get; set; } = 0.19m;
    public decimal PorcentajeICA { get; set; }
    public decimal ImpuestoEspecifico { get; set; }
    public decimal ArancelImportacion { get; set; }
    public bool ExentoIVA { get; set; }
    public bool GravadoICA { get; set; }
    public string? CodigoTributario { get; set; }

    // === CARACTERÍSTICAS FÍSICAS ===
    public decimal Peso { get; set; } = 0m;
    public decimal Volumen { get; set; } = 0m;
    public string? Dimensiones { get; set; }
    public bool EsPerecedero { get; set; }
    public bool RequiereSerial { get; set; }         // Indica si se debe instanciar UnidadProducto
    public string UnidadMedida { get; set; } = "unidades";
    
    public int? SupplierId { get; set; }
    public string? ImagenUrl { get; set; }
    public string? Notas { get; set; }
    public string? Tags { get; set; }
    public ProductoBaseStatus BaseStatus { get; set; }
    
    /// <summary>
    /// Proveedores de un producto.
    /// </summary>
    public ICollection<ProductoProveedor> Proveedores { get; set; }
        = new List<ProductoProveedor>();
    
    /// <summary>
    /// Variantes ligadas al producto.
    /// </summary>
    public ICollection<ProductoVariante> Variantes { get; set; } = new List<ProductoVariante>();
    
    
    public ICollection<ProductoBaseCategory> Categorias { get; set; } = new List<ProductoBaseCategory>();

    public int MarcaId { get; set; }
    public Marca? Marca { get; set; }
    
    // === HELPERS ===
    public bool TieneVariantes => Variantes.Count > 0;
    
}