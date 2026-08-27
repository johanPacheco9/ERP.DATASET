using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Caja.Requests;

/// <summary>
/// Datos requeridos para registrar y crear una nueva caja física (terminal POS) en el sistema.
/// </summary>
public sealed class CreateCajaRequest : IValidatableObject
{
    [Required(ErrorMessage = "El nombre de la caja es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El código interno es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Debe asociar la caja a una tienda.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la tienda debe ser válido.")]
    public int StoreId { get; set; }

    [Required(ErrorMessage = "Debe asociar la caja a un almacén.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del almacén debe ser válido.")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "El prefijo de facturación es obligatorio.")]
    [MaxLength(10, ErrorMessage = "El prefijo no puede superar los 10 caracteres.")]
    public string Prefix { get; set; } = null!;

    [Required(ErrorMessage = "El consecutivo inicial es obligatorio.")]
    [Range(1, long.MaxValue, ErrorMessage = "El consecutivo actual debe comenzar al menos en 1.")]
    public long CurrentConsecutive { get; set; } = 1;

    [MaxLength(50, ErrorMessage = "El número de resolución DIAN no puede superar los 50 caracteres.")]
    public string? DianResolutionNumber { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Validación de reglas de negocio cruzadas (ej. validaciones adicionales del prefijo o formato).
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Prefix) && Prefix != Prefix.ToUpperInvariant())
        {
            yield return new ValidationResult(
                "El prefijo de la caja generalmente debe estar en mayúsculas.",
                [nameof(Prefix)]
            );
        }
    }
}