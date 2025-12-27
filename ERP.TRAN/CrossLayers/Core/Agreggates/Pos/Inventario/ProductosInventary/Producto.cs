using ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;

public class Producto : EntityWithtraceability
{
    public string Codigo { get; set; } = null!; // SKU base
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    // Costos y precios base (referenciales, pueden variar por variante)
    public decimal Costo_Unitario { get; set; } = 0m;
    public decimal Precio_Venta { get; set; } = 0m;

    // Impuestos base
    public decimal PorcentajeIVA { get; set; } = 0.19m;
    public decimal PorcentajeICA { get; set; }
    public decimal ImpuestoEspecifico { get; set; }
    public decimal ArancelImportacion { get; set; }

    // Categorización fiscal
    public bool ExentoIVA { get; set; }
    public bool GravadoICA { get; set; }
    public string? CodigoTributario { get; set; }

    // Relaciones
    public int CategoriaId { get; set; }
    public int? ProveedorId { get; set; }

    // Unidad de medida
    public string Unidad_Medida { get; set; } = "unidades";

    // Atributos físicos genéricos
    public decimal Peso { get; set; } = 0m;
    public decimal Volumen { get; set; } = 0m;
    public string? Dimensiones { get; set; }

    public bool Es_Perecedero { get; set; }

    // Metadatos
    public string? Imagen_Url { get; set; }
    public string? Notas { get; set; }
    public string? Tags { get; set; }

    // Relaciones
    public Categoria Categoria { get; set; } = null!;
    public Proveedor Proveedor { get; set; } = null!;

    // Estado del producto
    public ProductoEnumStatus Estado { get; set; } = ProductoEnumStatus.Activo;

    public ICollection<ProductoVariante> Variantes { get; set; } = new List<ProductoVariante>();
    public ICollection<StockBodega> StockEnBodegas { get; set; } = new List<StockBodega>();
}
