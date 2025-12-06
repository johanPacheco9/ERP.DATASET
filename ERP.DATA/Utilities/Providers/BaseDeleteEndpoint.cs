using Ardalis.ApiEndpoints;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public abstract class BaseDeleteEndpoint<TRequest, TClass, TService>(TService service)
    : EndpointBaseAsync.WithRequest<TRequest>.WithActionResult
    where TRequest : IValidatableRequest
{
    protected const string OperationId = "Eliminar";

    protected readonly TService Service = service;

    protected ILogger<TClass> Logger { get; init; }

    public BaseDeleteEndpoint(TService service, ILogger<TClass> logger) : this(service)
    {
        Logger = logger;
    }

    public override async Task<ActionResult> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryDeleteEntity(request, cancellationToken));
    }

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

    protected abstract Task<ActionResult> DeleteEntity(
        TRequest request, CancellationToken cancellationToken);

    protected ActionResult EntityNotFound(string entityName, Guid identifier)
    {
        var error =
            $"No fue posible eliminar la entidad: {entityName} con id: {identifier} pues no se encontró en el service.";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    protected void TraceDeleted(string entityName, Guid primaryKey)
    {
        Logger.LogTrace($"La entidad '{entityName}', con PK: {primaryKey}, se eliminó satisfactoriamente.");
    }
}
