using System.ComponentModel;
using Ardalis.GuardClauses;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public class ListMovementsRequest : BaseListRequest, IValidatableRequest
{
    /// <summary>
    ///     Crea una solicitud ordenando los abonos por fecha de creación, del más reciente al más antiguo (descendente).
    /// </summary>
    /// <remarks>
    ///     Son requeridos los constructores sin parámetros para la deserialización de JSON.
    /// </remarks>
    public ListMovementsRequest()
    {
        OrderBy = $"{nameof(MovimientoDetailDto.MovimientoId)} desc";
    }

    /// <summary>
    ///     Crea una solicitud ordenando los abonos por fecha de creación, del más reciente al más antiguo (descendente).
    /// </summary>
    /// <param name="pageNumber">Número de página (comienza en 1)</param>
    /// <param name="pageSize">Tamaño de página (opcional), si es -1 no impone límites (tener precaución)</param>
    /// <param name="minDate">Fecha mínima de creación del cupón</param>
    /// <param name="maxDate">Fecha máxima de creación del cupón</param>
    /// <param name="orderBy">Criterio de ordenamiento</param>
    public ListMovementsRequest(
        int pageNumber, int pageSize, int? storeId,
        DateTime? minDate = null, DateTime? maxDate = null, string? orderBy = null)
    {
        OrderBy = orderBy ?? OrderBy;
        PageNumber = Guard.Against.Expression(i => i < 1, pageNumber, "Inválido");
        PageSize = pageSize;
        MinDate = minDate;
        MaxDate = maxDate;
        StoreId = storeId;
    }

    /// <summary>
    ///     Fecha mínima de creación (opcional)
    /// </summary>
    [DisplayName("Filtro: Mínima fecha de versión")]
    public DateTime? MinDate { get; set; }

    /// <summary>
    ///     Fecha máxima de creación (opcional)
    /// </summary>
    [DisplayName("Filtro: Máxima fecha de versión")]
    public DateTime? MaxDate { get; set; }

    /// <summary>
    /// Para filtrar por tienda.
    /// </summary>
    public int? StoreId { get; set; }

    /// <inheritdoc />
    public override bool ParametersAreValid(out string? errors)
    {
        var errorList = new List<string>();

        // Validación de paginación
        if (PageNumber < 1)
            errorList.Add("El número de página debe ser mayor o igual a 1.");

        if (PageSize == 0)
            errorList.Add("El tamaño de página no puede ser 0. Usa -1 si deseas obtener todos los registros sin límite.");

        // Validación de rango de fechas
        if (MinDate.HasValue && MaxDate.HasValue && MinDate > MaxDate)
            errorList.Add("La fecha mínima no puede ser mayor que la fecha máxima.");

        if (!string.IsNullOrWhiteSpace(OrderBy))
        {
            var validFields = new[]
            {
                nameof(MovimientoDetailDto.MovimientoId)
            };

            // Verifica si el campo de ordenamiento contiene alguno de los campos válidos
            if (!validFields.Any(v => OrderBy.Contains(v, StringComparison.OrdinalIgnoreCase)))
            {
                errorList.Add($"El campo de ordenamiento '{OrderBy}' no es válido. Campos permitidos: {string.Join(", ", validFields)}.");
            }
        }
        errors = errorList.Any() ? string.Join("; ", errorList) : null;

        return string.IsNullOrEmpty(errors);
    }
}