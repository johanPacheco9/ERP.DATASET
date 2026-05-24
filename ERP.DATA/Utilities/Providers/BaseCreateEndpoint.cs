using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Utilities.Providers;

/// <summary>
///     Endpoint base para operaciones de creación de entidades usando services.
/// </summary>
/// <typeparam name="TRequest">Tipo de solicitud</typeparam>
/// <typeparam name="TClass">Clase concreta del endpoint</typeparam>
public abstract class BaseCreateEndpoint<TRequest, TClass> : EndpointBaseAsync
    .WithRequest<TRequest>
    .WithActionResult
    where TRequest : IValidatableRequest
{
    /// <summary>
    ///     Identificador de operación.
    /// </summary>
    protected const string OperationId = "Crear";

    /// <summary>
    ///     Motor de log (inyectado).
    /// </summary>
    protected ILogger<TClass> Logger { get; }

    /// <summary>
    ///     Constructor protegido para inyección de dependencias.
    /// </summary>
    /// <param name="logger">Logger inyectado</param>
    protected BaseCreateEndpoint(ILogger<TClass> logger)
    {
        Logger = Guard.Against.Null(logger);
    }

    /// <summary>
    ///     Manejador principal de la solicitud de creación. Primero valida y luego ejecuta.
    /// </summary>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>ActionResult con el resultado de la operación</returns>
    public override async Task<ActionResult> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryCreateEntity(request, cancellationToken));
    }

    /// <summary>
    ///     Intenta crear la entidad usando el service correspondiente.
    /// </summary>
    /// <param name="request">Solicitud de creación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>ActionResult con el resultado de la operación</returns>
    private async Task<ActionResult> TryCreateEntity(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await CreateEntity(request, cancellationToken);
        }
        catch (Exception ex)
        {
            var error = $"No fue posible guardar la entidad: {ex.Message}{ex.InnerException?.Message}";
            Logger.LogError(error);
            return StatusCode(500, error);
        }
    }

    /// <summary>
    ///     Implementación concreta de creación usando services.
    ///     Aquí es donde el endpoint concreto llama al service correspondiente.
    /// </summary>
    /// <param name="request">Solicitud de creación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>ActionResult con el resultado</returns>
    protected abstract Task<ActionResult> CreateEntity(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Registra en el log que la entidad se creó correctamente.
    /// </summary>
    /// <param name="entityName">Name de la entidad creada</param>
    /// <param name="primaryKey">PK de la entidad creada</param>
    protected void TraceCreated(string entityName, int primaryKey)
    {
        Logger.LogTrace($"La entidad '{entityName}' se agregó satisfactoriamente, con PK: {primaryKey}");
    }
}