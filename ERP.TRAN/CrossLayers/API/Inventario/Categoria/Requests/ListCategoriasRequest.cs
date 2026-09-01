using Ardalis.GuardClauses;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;

public sealed class ListCategoriasRequest : BaseListRequest, IValidatableRequest
{
    /// <summary>
    ///     Crea una solicitud ordenando los abonos por fecha de creación, del más reciente al más antiguo (descendente).
    /// </summary>
    /// <remarks>
    ///     Son requeridos los constructores sin parámetros para la deserialización de JSON.
    /// </remarks>

    public ListCategoriasRequest()
    {
        OrderBy = $"{nameof(CategoriaDetailDto.Id)} desc";
    }

    /// <summary>
    ///     Crea una solicitud ordenando las categorias por fecha de creación, del más reciente a más antigua (descendente).
    /// </summary>
    /// <param name="pageNumber">Número de página (comienza en 1)</param>
    /// <param name="pageSize">Tamaño de página (opcional), si es -1 no impone límites (tener precaución)</param>
    /// <param name="minDate">Fecha mínima de creación del cupón</param>
    /// <param name="maxDate">Fecha máxima de creación de la categoria</param>
    /// <param name="orderBy">Criterio de ordenamiento</param>
    public ListCategoriasRequest(int pageNumber, int pageSize,
            DateTime? minDate = null, DateTime? maxDate = null, string? orderBy = null)
    {
        OrderBy = orderBy ?? OrderBy;
        PageNumber = Guard.Against.Expression(i => i < 1, pageNumber, "Inválido");
        PageSize = pageSize;
        MinDate = minDate;
        MaxDate = maxDate;

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


    public string? Search { get; set; }

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

        // Validación de campo de ordenamiento
        if (!string.IsNullOrWhiteSpace(OrderBy))
        {
            var validFields = new[]
            {
            nameof(CategoriaDetailDto.Id),
            nameof(CategoriaDetailDto.Nombre),
            nameof(CategoriaDetailDto.Descripcion)
        };

            // Verifica si el campo de ordenamiento contiene alguno de los campos válidos
            if (!validFields.Any(v => OrderBy.Contains(v, StringComparison.OrdinalIgnoreCase)))
            {
                errorList.Add($"El campo de ordenamiento '{OrderBy}' no es válido. Campos permitidos: {string.Join(", ", validFields)}.");
            }
        }

        // Resultado final
        errors = errorList.Any() ? string.Join("; ", errorList) : null;
        return string.IsNullOrEmpty(errors);
    }
}
