using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Request;
public sealed class CreateProductoVarianteRequest : IValidatableRequest
{
    public int ProductoId { get; set; }
    [Required]
    [MaxLength(50)]
    public string CodigoVariante { get; set; } = null!; // SKU

    [MaxLength(50)]
    public string? CodigoBarras { get; set; }

    [MaxLength(50)]
    public string? Lote { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PrecioVenta { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CostoUnitario { get; set; }
    
    public int? BodegaId { get; set; } //Se usa la bodea principal si no se especifica

    /// <summary>
    /// JSON o texto con atributos personalizados: {"Color": "Rojo", "Talla": "M"}
    /// </summary>
    public string? Atributos { get; set; }

    public List<CreateProductoVarianteRequest> variantesRequest { get; set; } = [];

    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (CodigoVariante == null)
        {
            errors = "El código de la variante es obligatorio.";
            return false;
        }

        if (CostoUnitario<0)
        {
            errors = "El costo unitario no puede ser negativo.";
            return false;
        }

        if (PrecioVenta < 0)
        {
            errors = "El precio de venta no puede ser negativo.";
            return false;
        }

        return true;
    }
}
