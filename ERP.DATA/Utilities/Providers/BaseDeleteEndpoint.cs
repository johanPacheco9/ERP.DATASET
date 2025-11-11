using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Utilities.Providers;

/// <summary>
/// Base para endpoints de eliminación de entidades usando services.
/// </summary>
/// <typeparam name="TRequest">Tipo de solicitud</typeparam>
/// <typeparam name="TClass">Clase concreta del endpoint</typeparam>
public abstract class BaseDeleteEndpoint<TRequest, TClass>(IServiceProvider serviceProvider) :
    EndpointBaseAsync.WithRequest<TRequest>.WithActionResult
    where TRequest : IValidatableRequest
{
    /// <summary>
    /// Identificador de operación
    /// </summary>
    protected const string OperationId = "Eliminar";

    /// <summary>
    /// Motor de log (inyectado)
    /// </summary>
    protected ILogger<TClass> Logger { get; init; } =
        Guard.Against.Null(serviceProvider.GetRequiredService<ILogger<TClass>>());

    /// <summary>
    /// Manejador principal de la solicitud de eliminación. Primero valida y luego ejecuta.
    /// </summary>
    public override async Task<ActionResult> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryDeleteEntity(request, cancellationToken));
    }

    /// <summary>
    /// Intenta eliminar la entidad usando el service correspondiente.
    /// Captura excepciones y devuelve 500 en caso de error.
    /// </summary>
    private async Task<ActionResult> TryDeleteEntity(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await DeleteEntity(request, cancellationToken);
        }
        catch (Exception ex)
        {
            var error = $"No fue posible eliminar la entidad: {ex.Message}{ex.InnerException?.Message}";
            Logger.LogError(error);
            return StatusCode(500, error);
        }
    }

    /// <summary>
    /// Implementación concreta de eliminación usando services.
    /// Aquí el endpoint concreto llama al service correspondiente.
    /// </summary>
    protected abstract Task<ActionResult> DeleteEntity(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Si no se encuentra la entidad, retorna 404 y registra en el log
    /// </summary>
    protected ActionResult EntityNotFound(string entityName, Guid identifier)
    {
        var error =
            $"No fue posible eliminar la entidad: {entityName} con id: {identifier} pues no se encontró en el service.";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    /// <summary>
    /// Registra en el log que la entidad se eliminó correctamente
    /// </summary>
    protected void TraceDeleted(string entityName, Guid primaryKey)
    {
        Logger.LogTrace($"La entidad '{entityName}', con PK: {primaryKey}, se eliminó satisfactoriamente.");
    }
}
