using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
public class RegistrarMovimientoSalidaRequest : IValidatableRequest
{
    [DisplayName("El id del producto a retirar")]
    [Required(ErrorMessage = "El ProductoId es obligatorio")]
    public Guid ProductoId { get; set; }

    [DisplayName("El id de la bodega a sacar del stock")]
    [Required(ErrorMessage = "El BodegaId es obligatorio")]
    public Guid BodegaId { get; set; }

    [DisplayName("Cantidad de productos a retirar")]
    [Required(ErrorMessage = "La cantidad para un movimiento de salida debe ser al menos de 1")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo unitario no puede ser negativo")]
    public decimal CostoUnitario { get; set; }

    // Referencias a otros módulos (opcionales)
    public Guid? ReferenciaId { get; set; }
    public string? ReferenciaTipo { get; set; } // 'venta','consumo','ajuste', etc.

    // Lote / vencimiento
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // Motivo y observaciones
    public string Motivo { get; set; } = "Salida de inventario";
    public string? Observaciones { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        var errorList = new List<string>();

        if (ProductoId == Guid.Empty)
            errorList.Add("El ProductoId es obligatorio");

        if (BodegaId == Guid.Empty)
            errorList.Add("El BodegaId es obligatorio");

        if (Cantidad <= 0)
            errorList.Add("La cantidad debe ser mayor a 0");

        if (CostoUnitario < 0)
            errorList.Add("El costo unitario no puede ser negativo");

        errors = errorList.Any() ? string.Join("; ", errorList) : null;
        return errors == null;
    }
}