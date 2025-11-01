using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;

public sealed class CreateProductoRequest : BaseCreateRequest, IValidatableRequest
{
    // ──────────────── Identificación ────────────────
    [Required(ErrorMessage = "El código del producto es obligatorio.")]
    [MaxLength(12, ErrorMessage = "El código no puede tener más de 12 caracteres.")]
    [MinLength(3, ErrorMessage = "El código debe tener al menos 3 caracteres.")]
    public string Codigo { get; set; } = null!;

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [MaxLength(120, ErrorMessage ="El nombre del producto no puede superar los 120 caracteres")]
    public string Nombre { get; set; } = null!;

    [StringLength(500, ErrorMessage ="La descripción no puede contener más de 500 caracteres")]
    public string? Descripcion { get; set; }

    // ──────────────── Precios y Costos base ────────────────
    [Range(0, double.MaxValue, ErrorMessage = "El costo unitario no puede ser negativo.")]
    public decimal Costo_Unitario { get; set; } = 0m;

    [Range(0, double.MaxValue, ErrorMessage = "El precio de venta no puede ser negativo.")]
    public decimal Precio_Venta { get; set; } = 0m;

    // ──────────────── Impuestos ────────────────
    [Range(0, 1, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 1.")]
    public decimal PorcentajeIVA { get; set; } = 0.19m;

    [Range(0, 1, ErrorMessage = "El porcentaje de ICA debe estar entre 0 y 1.")]
    public decimal PorcentajeICA { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ImpuestoEspecifico { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ArancelImportacion { get; set; }

    public bool ExentoIVA { get; set; }
    public bool GravadoICA { get; set; }

    [MaxLength(50)]
    public string? CodigoTributario { get; set; }

    // ──────────────── Relaciones ────────────────
    [Required(ErrorMessage = "Debe asignar una categoría al producto.")]
    public Guid CategoriaId { get; set; }

    public Guid? ProveedorId { get; set; }

    // ──────────────── Unidad y Atributos físicos ────────────────
    [MaxLength(50)]
    public string Unidad_Medida { get; set; } = "unidades";

    [Range(0, double.MaxValue)]
    public decimal Peso { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Volumen { get; set; }

    [MaxLength(100)]
    public string? Dimensiones { get; set; }

    // ──────────────── Metadatos ────────────────
    [MaxLength(500)]
    public string? Imagen_Url { get; set; }

    [MaxLength(500)]
    public string? Notas { get; set; }

    [MaxLength(200)]
    public string? Tags { get; set; }

    // ──────────────── Variantes ────────────────
    public List<CreateProductoVarianteRequest>? Variantes { get; set; }

    // ──────────────── Validación personalizada ────────────────
   
    public override bool ParametersAreValid(out string? errors)
    {
        errors = null;

        if (string.IsNullOrWhiteSpace(Codigo))
        {
            errors = "El código del producto es obligatorio.";
            return false;
        }

        if (Codigo.Length < 3 || Codigo.Length > 12)
        {
            errors = "El código debe tener entre 3 y 12 caracteres.";
            return false;
        }

        if (CategoriaId == Guid.Empty)
        {
            errors = "Debe asignar una categoría al producto.";
            return false;
        }

        if (Precio_Venta < 0 || Costo_Unitario < 0)
        {
            errors = "El costo o el precio no pueden ser negativos.";
            return false;
        }

        return true;
    }
}
