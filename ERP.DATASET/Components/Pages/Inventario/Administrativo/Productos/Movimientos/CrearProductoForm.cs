namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos.Movimientos;

using System.ComponentModel.DataAnnotations;

public class CrearProductoForm
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(12, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 12 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, MinimumLength = 4, ErrorMessage = "El nombre debe tener entre 4 y 120 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo no puede ser negativo.")]
    public decimal Costo_Unitario { get; set; } = 0m;

    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Precio_Venta { get; set; } = 0m;

    [Range(0, 1, ErrorMessage = "El IVA debe estar entre 0 y 1.")]
    public decimal PorcentajeIVA { get; set; } = 0.19m;

    [Range(0, 1, ErrorMessage = "El ICA debe estar entre 0 y 1.")]
    public decimal PorcentajeICA { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ImpuestoEspecifico { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ArancelImportacion { get; set; }

    public bool ExentoIVA { get; set; }
    public bool GravadoICA { get; set; }

    [StringLength(50)]
    public string? CodigoTributario { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una categoría.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
    public int CategoriaId { get; set; }

    public int? ProveedorId { get; set; }

    public string Unidad_Medida { get; set; } = "UND";

    [Range(0, double.MaxValue)]
    public decimal Peso { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Volumen { get; set; }

    [StringLength(100)]
    public string? Dimensiones { get; set; }

    // Sin MinLength — es opcional
    [StringLength(500, ErrorMessage = "La URL no puede superar los 500 caracteres.")]
    public string? Imagen_Url { get; set; }

    [StringLength(150)]
    public string? Notas { get; set; }

    [StringLength(150)]
    public string? Tags { get; set; }

    public bool Es_Perecedero { get; set; }
    public DateTime? FechaCaducidad { get; set; }
    public bool HasVariantes { get; set; }
}