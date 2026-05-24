using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public sealed class CreateExitMovementRequest : IValidatableRequest
{
    [DisplayName("Id de línea de producto (catálogo)")]
    public int LineaProductoId { get; set; }

    [DisplayName("Id de línea de producto (alias legacy)")]
    public int ProductoVarianteId { get; set; }

    [DisplayName("El id de la bodega de donde sale el stock")]
    [Required(ErrorMessage = "El WarehouseId es obligatorio")]
    public int BodegaId { get; set; }

    [DisplayName("Quantity de productos a retirar")]
    [Required(ErrorMessage = "La cantidad para un movimiento de salida debe ser al menos de 1")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    [DisplayName("Motive de la salida")]
    [Required(ErrorMessage = "El motivo de la salida es obligatorio")]
    [MaxLength(500)]
    public string Motivo { get; set; } = null!;

    [DisplayName("Observations adicionales")]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Validaciones de negocio (no solo anotaciones)
    /// </summary>
    public bool ParametersAreValid(out string? errors)
    {
        var errorList = new List<string>();

        if (LineaProductoId <= 0 && ProductoVarianteId <= 0)
            errorList.Add("Indique LineaProductoId o ProductoVarianteId");

        if (BodegaId <= 0)
            errorList.Add("El WarehouseId es obligatorio");

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
