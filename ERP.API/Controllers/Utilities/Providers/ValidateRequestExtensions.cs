using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Utilities.Providers;
public static class ValidatableRequestExtensions
{
    /// <summary>
    ///     Solo ejecuta la solicitud en caso de que sea válida.
    /// </summary>
    /// <param name="request">Solicitud a validar</param>
    /// <param name="logger">Log de registro</param>
    /// <param name="next">Función a ejecutar si es válida la solicitud</param>
    /// <typeparam name="T">Tipo donde se incorpora la extensión</typeparam>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    public static async Task<ActionResult> ValidateAndHandle<T>(this IValidatableRequest request, ILogger<T> logger, Func<Task<ActionResult>> next)
    {
        if (request.ParametersAreValid(out var errors))
            return await next();

        var errorMessage = $"Error validando la solicitud: {errors}.";
        logger.LogError(errorMessage);
        return new BadRequestObjectResult(errorMessage);
    }

    /// <summary>
    ///     Solo ejecuta la solicitud en caso de que sea válida.
    /// </summary>
    /// <param name="request">Solicitud a validar</param>
    /// <param name="logger">Log de registro</param>
    /// <param name="next">Función a ejecutar si es válida la solicitud</param>
    /// <typeparam name="T">Tipo donde se incorpora la extensión</typeparam>
    /// <typeparam name="TResponse">Tipo a devolver por la función a ejecutar si se valida la solicitud</typeparam>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    public static async Task<ActionResult<TResponse>> ValidateAndHandle<T, TResponse>(this IValidatableRequest request, ILogger<T> logger, Func<Task<ActionResult<TResponse>>> next)
    {
        if (request.ParametersAreValid(out var errors))
            return await next();

        var errorMessage = $"Error validando la solicitud: {errors}.";
        logger.LogError(errorMessage);
        return new BadRequestObjectResult(errorMessage);
    }
}