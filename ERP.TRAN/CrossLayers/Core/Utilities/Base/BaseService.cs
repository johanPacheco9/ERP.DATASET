using Ardalis.GuardClauses;
using ERP.TRAN.CrossLayers.Core.Utilities.Structs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Base;

public abstract class BaseService<T> where T : class
{

    protected BaseService(ILogger<T> logger,IHttpContextAccessor httpCtx)
    {
        Log = Guard.Against.Null(logger);
        _httpCtx = Guard.Against.Null(httpCtx);
    }

    private readonly IHttpContextAccessor _httpCtx;

    protected ILogger<T> Log { get; set; }
    private async Task<string?> GetAccessToken()
    {
        // Accede al contexto de autenticación para obtener el token de acceso
        Log.LogTrace("Obteniendo contexto HTTP");
        var context = _httpCtx.HttpContext;
        if (context != null)
        {
            Log.LogTrace("Obteniendo token de acceso desde el contexto HTTP");
            var token = await context.GetTokenAsync("access_token");
            Log.LogTrace("Se obtuvo el token de acceso desde el contexto HTTP");
            return token;
        }

        Log.LogTrace("NO se obtuvo el contexto HTTP");
        return null;
    }

    protected async Task<HttpClient> InitApiClient()
    {
        Log.LogTrace("Obteniendo token de acceso");
        var token = await GetAccessToken();
        if (token != null)
        {
            Log.LogTrace("Preparando cliente HTTP con token de acceso");
            return _api.Prepare(token);
        }

        Log.LogTrace("No se pudo obtener el token de acceso");
        throw new Exception();
    }

    protected static PaginationHeaders ProcessPaginationHeaders(HttpResponseMessage res)
    {
        var header = res.Headers
            .Where(h => h.Key == "X-Pagination")
            .Select(s => s.Value).First().First();

        return JsonConvert.DeserializeObject<PaginationHeaders>(header);
    }
}

