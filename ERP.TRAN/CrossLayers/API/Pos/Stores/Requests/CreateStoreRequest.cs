using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;

/// <summary>
/// Objeto de transferencia de datos para la creación de una nueva tienda o sucursal.
/// </summary>
public sealed class CreateStoreRequest
{
    [Required(ErrorMessage = "El nombre de la tienda es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Name { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Description { get; set; }

    /// <summary>
    /// Indica si esta sucursal fungirá como la tienda principal del sistema.
    /// </summary>
    public bool IsMainStore { get; set; }

    /// <summary>
    /// Define si la tienda se encuentra activa para operar desde el momento de su creación.
    /// </summary>
    public bool IsActive { get; set; } = true;
}