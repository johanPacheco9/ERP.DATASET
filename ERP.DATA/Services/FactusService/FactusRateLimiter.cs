using System.Threading.RateLimiting;

namespace ERP.DATA.Services.FactusService;

/// <summary>
/// Limitador de tasa compartido para toda la aplicación (80 req/min según Factus).
/// Singleton para compartir entre circuitos/servicios.
/// </summary>
public sealed class FactusRateLimiter : IDisposable
{
    private readonly TokenBucketRateLimiter _limiter;

    public FactusRateLimiter()
    {
        _limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 80,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 100,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 80,
            AutoReplenishment = true
        });
    }

    public async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        using var lease = await _limiter.AcquireAsync(1, cancellationToken);
        if (!lease.IsAcquired)
            throw new FactusRateLimitException("Límite de tasa de Factus alcanzado.");
    }

    public void Dispose() => _limiter.Dispose();
}
