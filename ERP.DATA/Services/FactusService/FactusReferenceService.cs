using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ERP.DATA.Services.FactusService;

public sealed class FactusReferenceService
{
    private readonly HttpClient _httpClient;
    private readonly IFactusTokenProvider _tokenProvider;
    private readonly FactusRateLimiter _rateLimiter;
    private readonly FactusOptions _options;
    private readonly IMemoryCache _cache;

    public FactusReferenceService(HttpClient httpClient, IFactusTokenProvider tokenProvider, FactusRateLimiter rateLimiter, IOptions<FactusOptions> options, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _rateLimiter = rateLimiter;
        _options = options.Value;
        _cache = cache;
        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
    }

    public async Task<List<FactusNumberingRange>> ObtenerRangosAsync(CancellationToken cancellationToken = default)
    {
        const string key = "factus:numbering-ranges";
        if (_cache.TryGetValue(key, out List<FactusNumberingRange>? cached) && cached is not null)
            return cached;

        await _rateLimiter.WaitAsync(cancellationToken);
        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        using var http = new HttpRequestMessage(HttpMethod.Get, "/v2/numbering-ranges");
        http.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var resp = await _httpClient.SendAsync(http, cancellationToken);
        if (!resp.IsSuccessStatusCode) return new List<FactusNumberingRange>();

        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        using var json = System.Text.Json.JsonDocument.Parse(body);
        var data = json.RootElement.TryGetProperty("data", out var d) ? d : json.RootElement;
        var list = new List<FactusNumberingRange>();
        if (data.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var el in data.EnumerateArray())
            {
                list.Add(new FactusNumberingRange
                {
                    Id = el.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                    Document = el.TryGetProperty("document", out var doc) ? doc.GetString() : null,
                    Prefix = el.TryGetProperty("prefix", out var p) ? p.GetString() : null,
                    From = el.TryGetProperty("from", out var f) ? f.GetInt32() : 0,
                    To = el.TryGetProperty("to", out var t) ? t.GetInt32() : 0,
                    Current = el.TryGetProperty("current", out var c) ? c.GetInt32() : 0,
                    IsActive = el.TryGetProperty("is_active", out var a) && a.GetInt32() == 1,
                    IsExpired = el.TryGetProperty("is_expired", out var e) && e.GetInt32() == 1
                });
            }
        }

        _cache.Set(key, list, TimeSpan.FromMinutes(30));
        return list;
    }
}

public sealed class FactusNumberingRange
{
    public int Id { get; set; }
    public string? Document { get; set; }
    public string? Prefix { get; set; }
    public int From { get; set; }
    public int To { get; set; }
    public int Current { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
}
