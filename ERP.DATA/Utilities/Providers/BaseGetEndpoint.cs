using Ardalis.ApiEndpoints;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public abstract class BaseGetEndpoint<TRequest, TClass, TResponse> :
    EndpointBaseAsync.WithRequest<TRequest>.WithActionResult<TResponse>
    where TRequest : IValidatableRequest
{
    protected const string OperationId = "Consultar";

    protected ILogger<TClass> Logger { get; }

    protected BaseGetEndpoint(ILogger<TClass> logger)
    {
        Logger = logger;
    }

    public override async Task<ActionResult<TResponse>> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryGetEntity(request, cancellationToken));
    }

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

    protected abstract Task<ActionResult<TResponse>> GetEntity(TRequest request, CancellationToken cancellationToken);

    protected ActionResult EntityNotFound(string entityName,int  identifier)
    {
        var error = $"No se encontró la entidad: {entityName} con id: {identifier}";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    protected ActionResult EntityNotFound(string entityName)
    {
        var error = $"No se encontró la entidad: {entityName} según los criterios especificados";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    protected void TraceFound(string entityName, int primaryKey)
    {
        Logger.LogTrace($"La entidad '{entityName}' con PK: {primaryKey} se retornó satisfactoriamente.");
    }
}
