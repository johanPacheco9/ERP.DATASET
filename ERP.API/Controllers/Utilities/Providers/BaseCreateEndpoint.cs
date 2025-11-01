using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using ERP.API.Controllers.Utilities.Providers;
using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ERP.API.Controllers.Utilities.Base;




/// <summary>
///     Endpoint base para operaciones de creación de entidades.
/// </summary>
/// <param name="serviceProvider">Contenedor de dependencias</param>
/// <typeparam name="TRequest">Solicitud de eliminación</typeparam>
/// <typeparam name="TClass">Clase concreta</typeparam>
public abstract class BaseCreateEndpoint<TRequest, TClass>(IServiceProvider serviceProvider) :
    EndpointBaseAsync.WithRequest<TRequest>.WithActionResult where TRequest : IValidatableRequest
{
    /// <summary>
    ///     Identificador de operación.
    /// </summary>
    protected const string OperationId = "Crear";

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
    ///     Manejador de la solicitud de creación. Primero valida y luego ejecuta.
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
            async () => await TryCreateEntity(request, cancellationToken));
    }

    /// <summary>
    ///     Intenta crear la entidad. Si falla, se captura la excepción y se retorna un error 500.
    /// </summary>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    private async Task<ActionResult> TryCreateEntity(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await CreateEntity(request, cancellationToken);
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
    ///     Realiza la creación de la entidad.
    /// </summary>
    /// <param name="request">Solicitud</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns><c>ActionResult</c> con el resultado. Es <c>Awaitable</c></returns>
    /// <exception cref="DbUpdateException">Si hay problemas al ejecutar en el repositorio</exception>
    /// <exception cref="Exception">Si hay problemas al ejecutar en el repositorio o en el código</exception>
    protected abstract Task<ActionResult> CreateEntity(TRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Registra en el log que la entidad especificada fue creada satisfactoriamente.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad creada</param>
    /// <param name="primaryKey">PK de la entidad creada</param>
    protected void TraceCreated(string entityName, int primaryKey)
    {
        Logger.LogTrace($"La entidad: {entityName} se agregó satisfactoriamente, con PK: {primaryKey}");
    }
}