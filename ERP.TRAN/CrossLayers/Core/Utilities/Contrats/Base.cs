namespace ERP.TRAN.CrossLayers.Core.Utilities.Contracts;

/// <summary>
///     Contrato de una solicitud que puede ser validada.
/// </summary>
public interface IValidatableRequest
{
    /// <summary>
    ///     Devuelve si los parámetros de la solicitud son válidos.
    /// </summary>
    /// <param name="errors">Errores si no es válida la solicitud</param>
    /// <returns><c>true</c> si la solicitud es válida, o <c>false</c> de lo contrario</returns>
    bool ParametersAreValid(out string? errors);
}