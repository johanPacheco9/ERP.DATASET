using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public sealed class CreateEntryMovementRequest : IValidatableRequest
{
    [DisplayName("El id del producto a ingresar")]
    [Required(ErrorMessage = "El ProductoId es obligatorio")]
    public int ProductoVarianteId { get; set; }


    [DisplayName("El id de la bodega a agregar stock")]
    [Required(ErrorMessage = "El BodegaId es obligatorio")]
    public int BodegaId { get; set; }


    [DisplayName("Cantidad de productos a ingresar")]
    [Required(ErrorMessage = "La cantidad para un movimiento de entrada debe ser al menos de 1")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    // Motivo y observaciones
    public string Motivo { get; set; } = "Entrada de inventario";
    public string? Observaciones { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        var errorList = new List<string>();

        if (ProductoVarianteId == 0)
            errorList.Add("El ProductoId es obligatorio");

        if (BodegaId == 0)
            errorList.Add("El BodegaId es obligatorio");

        if (Cantidad <= 0)
            errorList.Add("La cantidad debe ser mayor a 0");

        errors = errorList.Any() ? string.Join("; ", errorList) : null;
        return errors == null;
    }
}

