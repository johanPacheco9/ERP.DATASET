
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
/// <summary>
///     Clase base para las solicitudes de consulta de entidades.
/// </summary>
public abstract class BaseListRequest : IValidatableRequest
{
    /// <summary>
    ///     El número de página (comienza en 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    ///     Tamaño de página (cantidad de resultados a devolver) de la consulta.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    ///     Campo por el cual se ordenarán los resultados.
    /// </summary>
    public string OrderBy { get; set; } = null!;

    /// <summary>
    ///     Si los parámetros de la solicitud son válidos.
    /// </summary>
    /// <param name="errors">Errores en la solicitud</param>
    /// <returns><c>true</c> si la solicitud es válida, o <c>false</c> de lo contrario</returns>
    public abstract bool ParametersAreValid(out string? errors);
}

