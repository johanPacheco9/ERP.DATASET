using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers.Utilities.Providers;

public abstract class BaseGetEndpoint<TRequest, TClass, TResponse>(IServiceProvider serviceProvider) :
    EndpointBaseAsync.WithRequest<TRequest>.WithActionResult<TResponse> where TRequest : IValidatableRequest
{
    /// <summary>
    ///     Identificador de operación.
    /// </summary>
    protected const string OperationId = "Consultar";

    /// <summary>
    ///     Repositorio (inyectado).
    /// </summary>
    protected MainDataContext Repository { get; init; } =
        Guard.Against.Null(serviceProvider.GetRequiredService<MainDataContext>());

    /// <summary>
    ///     Motor de log (inyectado).
    /// </summary>
    protected ILogger<TClass> Logger { get; init; } =
        Guard.Against.Null(serviceProvider.GetRequiredService<ILogger<TClass>>());

    /// <summary>
    ///     Manejador de la solicitud de consulta. Primero valida y luego ejecuta.
    /// </summary>
    /// <remarks>
    ///    Si la validación falla, se retorna un error 400 con los mensajes de validación.
    /// </remarks>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    public override async Task<ActionResult<TResponse>> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = new())
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryGetEntity(request, cancellationToken));
    }

    /// <summary>
    ///     Intenta consultar la entidad. Si falla, se captura la excepción y se retorna un error 500.
    /// </summary>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    private async Task<ActionResult<TResponse>> TryGetEntity(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await GetEntity(request, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var error = $"No fue posible guardar la entidad: {ex.Message + ex.InnerException?.Message}";
            Logger.LogError(error);
            return StatusCode(500, $"{error}");
        }
        catch (Exception ex)
        {
            var error = $"No fue posible guardar la entidad: {ex.Message}";
            Logger.LogError(error);
            return StatusCode(500, $"{error}");
        }
    }

    /// <summary>
    ///     Realiza la consulta de la entidad.
    /// </summary>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    /// <exception cref="DbUpdateException">Si hay problemas al ejecutar en el repositorio</exception>
    /// <exception cref="Exception">Si hay problemas al ejecutar en el repositorio o en el código</exception>
    protected abstract Task<ActionResult<TResponse>> GetEntity(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Si no se encuentra la entidad en el repositorio, se retorna un error 404 luego de registrar el evento.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad buscada (para el log)</param>
    /// <param name="identifier">PK de la entidad buscada (para el log)</param>
    /// <returns><c>NotFound</c> luego de registrar en el log</returns>
    protected ActionResult EntityNotFound(string entityName, Guid identifier)
    {
        var error = $"No fue posible encontrar la entidad: {entityName} con id: {identifier} pues no se encontró en el repositorio.";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    /// <summary>
    ///     Si no se encuentra la entidad en el repositorio, se retorna un error 404 luego de registrar el evento.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad buscada (para el log)</param>
    /// <returns><c>NotFound</c> luego de registrar en el log</returns>
    protected ActionResult EntityNotFound(string entityName)
    {
        var error = $"No fue posible encontrar la entidad: {entityName} con los criterios especificados, pues no se encontró en el repositorio.";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    /// <summary>
    ///     Registra en el log que la entidad especificada fue encontrada y retornada satisfactoriamente.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad encontrada</param>
    /// <param name="primaryKey">PK de la entidad encontrada</param>
    protected void TraceFound(string entityName, Guid primaryKey)
    {
        Logger.LogTrace($"La entidad: {entityName}, con PK: {primaryKey}, se retornó satisfactoriamente");
    }
}

