using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public sealed class CreateExitMovementRequest : IValidatableRequest
{
    [DisplayName("El id del producto a retirar")]
    [Required(ErrorMessage = "El ProductoVarianteId es obligatorio")]
    public int ProductoVarianteId { get; set; }

    [DisplayName("El id de la bodega de donde sale el stock")]
    [Required(ErrorMessage = "El BodegaId es obligatorio")]
    public int BodegaId { get; set; }

    [DisplayName("Cantidad de productos a retirar")]
    [Required(ErrorMessage = "La cantidad para un movimiento de salida debe ser al menos de 1")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    [DisplayName("Motivo de la salida")]
    [Required(ErrorMessage = "El motivo de la salida es obligatorio")]
    [MaxLength(500)]
    public string Motivo { get; set; } = null!;

    [DisplayName("Observaciones adicionales")]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Validaciones de negocio (no solo anotaciones)
    /// </summary>
    public bool ParametersAreValid(out string? errors)
    {
        var errorList = new List<string>();

        if (ProductoVarianteId <= 0)
            errorList.Add("El ProductoVarianteId es obligatorio");

        if (BodegaId <= 0)
            errorList.Add("El BodegaId es obligatorio");

        if (Cantidad <= 0)
            errorList.Add("La cantidad debe ser mayor a 0");

        if (string.IsNullOrWhiteSpace(Motivo))
            errorList.Add("El motivo es obligatorio");

        errors = errorList.Any()
            ? string.Join("; ", errorList)
            : null;

        return errors == null;
    }
}
