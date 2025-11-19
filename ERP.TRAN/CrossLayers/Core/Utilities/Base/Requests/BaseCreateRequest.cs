using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.Utilities.Base.Requests;

/// <summary>
///     Clase base para las solicitudes de creación de entidades.
/// </summary>
public abstract class BaseCreateRequest : IValidatableRequest
{
    /// <summary>
    ///     Creador de la entidad (metadato).
    /// </summary>
    [Required(ErrorMessage = "Debe indicar el usuario creador")]
    // ReSharper disable once InconsistentNaming
    public string _CreatorAuth0Id { get; set; } = null!;

    /// <summary>
    ///     Devuelve si los parámetros de la solicitud son válidos.
    /// </summary>
    /// <param name="errors">Errores si no es válida la solicitud</param>
    /// <returns><c>true</c> si la solicitud es válida, o <c>false</c> de lo contrario</returns>
    public abstract bool ParametersAreValid(out string? errors);
}