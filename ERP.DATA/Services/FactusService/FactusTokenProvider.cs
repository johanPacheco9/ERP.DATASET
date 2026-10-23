using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace ERP.DATA.Services.FactusService;

/// <summary>
/// Proveedor de token de acceso para Factus con renovación thread-safe.
/// Implementa un patrón "factory" con SemaphoreSlim para que, si varias peticiones
/// intentan renovar el token a la vez, solo la primera realice la llamada HTTP
/// y las demás esperen y reutilicen el token resultante.
/// </summary>
public sealed class FactusTokenProvider : IFactusTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly FactusOptions _options;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string? _accessToken;
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
        // Fast path: token vigente.
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt.AddSeconds(-30))
            return _accessToken!;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check dentro del lock.
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

    private async Task RenewAsync(CancellationToken cancellationToken)
    {
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
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<FactusTokenResponse>(cancellationToken: cancellationToken)
                    ?? throw new InvalidOperationException("Respuesta de token vacía.");

        _accessToken = token.AccessToken;
        // expires_in viene en segundos.
        _expiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn > 0 ? token.ExpiresIn : 600);
    }
}

public interface IFactusTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}

internal sealed class FactusTokenResponse
{
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
