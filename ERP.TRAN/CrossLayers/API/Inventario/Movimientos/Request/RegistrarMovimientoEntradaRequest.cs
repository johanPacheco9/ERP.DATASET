using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public class RegistrarMovimientoEntradaRequest : IValidatableRequest
{
    [DisplayName("El id de la variante de producto a ingresar")]
    [Required(ErrorMessage = "El ProductoVarianteId es obligatorio")]
    public int ProductoVarianteId { get; set; }

    [DisplayName("El id de la bodega a agregar stock")]
    [Required(ErrorMessage = "El BodegaId es obligatorio")]
    public int BodegaId { get; set; }

    [DisplayName("Cantidad de productos a ingresar")]
    [Required(ErrorMessage = "La cantidad para un movimiento de entrada debe ser al menos de 1")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public int Cantidad { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El costo unitario no puede ser negativo")]
    public decimal CostoUnitario { get; set; }

    // Referencias a otros módulos (opcionales)
    public int? ReferenciaId { get; set; }
    public string? ReferenciaTipo { get; set; } // 'compra', 'traslado', 'ajuste_manual', etc.

    // Lote / Vencimiento
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // Motivo y Observaciones
    public string Motivo { get; set; } = "Entrada de inventario";
    public string? Observaciones { get; set; }

    // === SERIALES / IMEIs ===
    public bool? RequiereSerial { get; set; }
    
    /// <summary>
    /// Colección de seriales o IMEIs individuales para cada unidad que ingresa.
    /// </summary>
    public List<string>? Seriales { get; set; } = new();

    public bool ParametersAreValid(out string? errors)
    {
        var errorList = new List<string>();

        if (ProductoVarianteId <= 0)
            errorList.Add("El ProductoVarianteId es obligatorio");

        if (BodegaId <= 0)
            errorList.Add("El BodegaId es obligatorio");

        if (Cantidad <= 0)
            errorList.Add("La cantidad debe ser mayor a 0");

        if (CostoUnitario < 0)
            errorList.Add("El costo unitario no puede ser negativo");

        // Validar coherencia entre Cantidad y Seriales
        if (RequiereSerial == true || (Seriales != null && Seriales.Any()))
        {
            if (Seriales == null || Seriales.Count != Cantidad)
            {
                errorList.Add($"Se especificaron {Seriales?.Count ?? 0} seriales, pero la cantidad del movimiento es {Cantidad}.");
            }

            if (Seriales != null && Seriales.Any(string.IsNullOrWhiteSpace))
            {
                errorList.Add("Uno o más seriales provistos están vacíos o son inválidos.");
            }
        }

        errors = errorList.Any() ? string.Join("; ", errorList) : null;
        return errors == null;
    }
}