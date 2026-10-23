using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ERP.DATA.Services.FactusService;

public sealed class FactusTokenProvider : IFactusTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly FactusOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _expiresAt = DateTime.MinValue;

    public FactusTokenProvider(HttpClient httpClient, IOptions<FactusOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt.AddSeconds(-30))
            return _accessToken!;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt.AddSeconds(-30))
                return _accessToken!;

            await RenewAsync(cancellationToken);
            return _accessToken!;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void InvalidateToken()
    {
        _accessToken = null;
        _expiresAt = DateTime.MinValue;
    }

    private async Task RenewAsync(CancellationToken cancellationToken)
    {
        // 1) Intentar refresh_token si existe.
        if (!string.IsNullOrEmpty(_refreshToken))
        {
            var refreshForm = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["refresh_token"] = _refreshToken!
            };
            using var refreshContent = new FormUrlEncodedContent(refreshForm);
            var refreshResponse = await _httpClient.PostAsync("/oauth/token", refreshContent, cancellationToken);
            if (refreshResponse.IsSuccessStatusCode)
            {
                var rt = await refreshResponse.Content.ReadFromJsonAsync<FactusTokenResponse>(cancellationToken: cancellationToken);
                if (rt != null && !string.IsNullOrEmpty(rt.AccessToken))
                {
                    _accessToken = rt.AccessToken;
                    _refreshToken = string.IsNullOrEmpty(rt.RefreshToken) ? _refreshToken : rt.RefreshToken;
                    _expiresAt = DateTime.UtcNow.AddSeconds(rt.ExpiresIn > 0 ? rt.ExpiresIn : 600);
                    return;
                }
            }
            _refreshToken = null;
        }

        // 2) Password grant.
        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["username"] = _options.Username,
            ["password"] = _options.Password
        };

        using var content = new FormUrlEncodedContent(form);
        var response = await _httpClient.PostAsync("/oauth/token", content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new FactusAuthException($"No se pudo obtener token de Factus: {(int)response.StatusCode} {body}");
        }

        var token = await response.Content.ReadFromJsonAsync<FactusTokenResponse>(cancellationToken: cancellationToken)
                    ?? throw new FactusAuthException("Respuesta de token vacía.");

        _accessToken = token.AccessToken;
        _refreshToken = string.IsNullOrEmpty(token.RefreshToken) ? _refreshToken : token.RefreshToken;
        _expiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn > 0 ? token.ExpiresIn : 600);
    }
}

public interface IFactusTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    void InvalidateToken();
}

internal sealed class FactusTokenResponse
{
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
