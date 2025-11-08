using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Utilities.Providers;

/// <summary>
/// Base para endpoints de consulta de entidades usando services.
/// </summary>
/// <typeparam name="TRequest">Tipo de solicitud</typeparam>
/// <typeparam name="TClass">Clase concreta del endpoint</typeparam>
/// <typeparam name="TResponse">Tipo de respuesta</typeparam>
public abstract class BaseGetEndpoint<TRequest, TClass, TResponse>(IServiceProvider serviceProvider) :
    EndpointBaseAsync.WithRequest<TRequest>.WithActionResult<TResponse>
    where TRequest : IValidatableRequest
{
    /// <summary>
    /// Identificador de operación
    /// </summary>
    protected const string OperationId = "Consultar";

    /// <summary>
    /// Logger inyectado
    /// </summary>
    protected ILogger<TClass> Logger { get; init; } =
        Guard.Against.Null(serviceProvider.GetRequiredService<ILogger<TClass>>());

    /// <summary>
    /// Maneja la solicitud de consulta: primero valida y luego ejecuta
    /// </summary>
    public override async Task<ActionResult<TResponse>> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryGetEntity(request, cancellationToken));
    }

    /// <summary>
    /// Intenta obtener la entidad usando el service correspondiente.
    /// Captura excepciones y retorna 500 si ocurre algún error.
    /// </summary>
    private async Task<ActionResult<TResponse>> TryGetEntity(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await GetEntity(request, cancellationToken);
        }
        catch (Exception ex)
        {
            var error = $"No fue posible consultar la entidad: {ex.Message}{ex.InnerException?.Message}";
            Logger.LogError(error);
            return StatusCode(500, error);
        }
    }

    /// <summary>
    /// Implementación concreta de consulta usando services
    /// </summary>
    protected abstract Task<ActionResult<TResponse>> GetEntity(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Retorna 404 si no se encuentra la entidad
    /// </summary>
    protected ActionResult EntityNotFound(string entityName, Guid identifier)
    {
        var error = $"No se encontró la entidad: {entityName} con id: {identifier}";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    /// <summary>
    /// Retorna 404 si no se encuentra la entidad según criterios
    /// </summary>
    protected ActionResult EntityNotFound(string entityName)
    {
        var error = $"No se encontró la entidad: {entityName} según los criterios especificados";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    /// <summary>
    /// Registra que la entidad fue encontrada exitosamente
    /// </summary>
    protected void TraceFound(string entityName, Guid primaryKey)
    {
        Logger.LogTrace($"La entidad '{entityName}' con PK: {primaryKey} se retornó satisfactoriamente.");
    }
}
