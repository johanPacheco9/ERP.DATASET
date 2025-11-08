using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using ERP.API.Controllers.Utilities.Providers;
using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Utilities.Providers;

/// <summary>
///     Endpoint base para operaciones de creación de entidades.
/// </summary>
/// <param name="serviceProvider">Contenedor de dependencias</param>
/// <typeparam name="TRequest">Solicitud de eliminación</typeparam>
/// <typeparam name="TClass">Clase concreta</typeparam>
public abstract class BaseUpdateEndpoint<TRequest, TClass>(IServiceProvider serviceProvider) :
    EndpointBaseAsync.WithRequest<TRequest>.WithActionResult where TRequest : IValidatableRequest
{
    /// <summary>
    ///     Identificador de operación.
    /// </summary>
    protected const string OperationId = "Actualizar";

    /// <summary>
    ///     Repositorio (inyectado).
    /// </summary>
    protected MainDataContext Repository { get; init; } =
        Guard.Against.Null(serviceProvider.GetRequiredService<MainDataContext>());

    /// <summary>
    ///     Repositorio para batch (inyectado).
    /// </summary>
    /// 
    /*
    protected BatchDbContext RepositoryBatch { get; init; } =
        Guard.Against.Null(serviceProvider.GetRequiredService<BatchDbContext>());
    */

    /// <summary>
    ///     Motor de log (inyectado).
    /// </summary>
    protected ILogger<TClass> Logger { get; init; } =
        Guard.Against.Null(serviceProvider.GetRequiredService<ILogger<TClass>>());

    /// <summary>
    ///     Manejador de la solicitud de actualización. Primero valida y luego ejecuta.
    /// </summary>
    /// <remarks>
    ///    Si la validación falla, se retorna un error 400 con los mensajes de validación.
    /// </remarks>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    public override async Task<ActionResult> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = new())
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryUpdateEntity(request, cancellationToken));
    }

    /// <summary>
    ///     Intenta crear la entidad. Si falla, se captura la excepción y se retorna un error 500.
    /// </summary>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    private async Task<ActionResult> TryUpdateEntity(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await UpdateEntity(request, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var error = $"No fue posible actualizar la entidad: {ex.Message + ex.InnerException?.Message}";
            Logger.LogError(error);
            return StatusCode(500, $"{error}");
        }
        catch (Exception ex)
        {
            var error = $"No fue posible actualizar la entidad: {ex.Message}";
            Logger.LogError(error);
            return StatusCode(500, $"{error}");
        }
    }

    /// <summary>
    ///     Realiza la creación de la entidad.
    /// </summary>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    /// <exception cref="DbUpdateException">Si hay problemas al ejecutar en el repositorio</exception>
    /// <exception cref="Exception">Si hay problemas al ejecutar en el repositorio o en el código</exception>
    protected abstract Task<ActionResult> UpdateEntity(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Si no se encuentra la entidad en el repositorio, se retorna un error 404 luego de registrar el evento.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad buscada (para el log)</param>
    /// <param name="identifier">PK de la entidad buscada (para el log)</param>
    /// <returns><c>NotFound</c> luego de registrar en el log</returns>
    protected ActionResult EntityNotFound(string entityName, int identifier)
    {
        var error =
            $"No fue posible actualizar la entidad: {entityName} con id: {identifier} pues no se encontró en el repositorio.";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    /// <summary>
    ///     Registra en el log que la entidad especificada fue actualizada satisfactoriamente.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad actualizada</param>
    /// <param name="primaryKey">PK de la entidad actualizada</param>
    protected void TraceUpdated(string entityName, int primaryKey)
    {
        Logger.LogTrace($"La entidad: {entityName}, con PK: {primaryKey}, se actualizó satisfactoriamente");
    }
}
