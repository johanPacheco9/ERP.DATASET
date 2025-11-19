using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using ERP.TRAN.CrossLayers.Core.Utilities.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ERP.DATA.Utilities.Providers;

public abstract class BaseListEndpoint<TRequest, TClass, TResponse>
    : EndpointBaseAsync.WithRequest<TRequest>.WithActionResult<TResponse>
    where TRequest : IValidatableRequest
{
    protected readonly ILogger<TClass> Logger;

    protected BaseListEndpoint(ILogger<TClass> logger)
    {
        Logger = Guard.Against.Null(logger);
    }

    public override async Task<ActionResult<TResponse>> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await request.ValidateAndHandle(Logger,
            async () => await TryListEntity(request, cancellationToken));
    }

    private async Task<ActionResult<TResponse>> TryListEntity(
        TRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ListEntity(request, cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            var error = $"No fue posible listar la entidad: {ex.Message + ex.InnerException?.Message}";
            Logger.LogError(error);
            return StatusCode(500, error);
        }
        catch (Exception ex)
        {
            var error = $"No fue posible listar la entidad: {ex.Message}";
            Logger.LogError(error);
            return StatusCode(500, error);
        }
    }

    protected abstract Task<ActionResult<TResponse>> ListEntity(
        TRequest request,
        CancellationToken cancellationToken);

    protected ActionResult EntityNotFound(string entityName, int identifier)
    {
        var error = $"No se encontró la entidad {entityName} con id {identifier}.";
        Logger.LogTrace(error);
        return NotFound(error);
    }

    protected void TraceListFiltered(string entityName, string field, string filter)
    {
        Logger.LogTrace($"Filtrando {entityName} por {field}: {filter}");
    }

    protected void TraceListFiltered(string entityName, string field, DateTime dateFilter)
    {
        Logger.LogTrace($"Filtrando {entityName} por {field}: {dateFilter:yyyy-MM-dd}");
    }

    protected void LogGeneratedQuery(IQueryable query)
    {
        Logger.LogInformation($"Query: {query.ToQueryString()}");
    }

    protected void PrepareResponseHeaders(PaginationHeaders paginationHeaders)
    {
        Response.Headers["X-Pagination"] = JsonConvert.SerializeObject(paginationHeaders);
    }
}
