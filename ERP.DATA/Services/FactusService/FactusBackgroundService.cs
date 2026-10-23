using ERP.DATA.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

namespace ERP.DATA.Services.FactusService;

/// <summary>
/// Worker que procesa la cola de documentos electrónicos (DocumentosElectronicos)
/// con estado Pendiente/Procesando. Un solo worker concurrente por el límite de
/// 80 req/min de Factus.
/// </summary>
public sealed class FactusBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FactusBackgroundService> _logger;

    public FactusBackgroundService(IServiceScopeFactory scopeFactory, ILogger<FactusBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Al arrancar, recuperar los que quedaron en Procesando.
        await ResetStaleProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOneAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en FactusBackgroundService loop");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ResetStaleProcessingAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MainDataContext>();
        var stale = await db.DocumentosElectronicos.Where(d => d.Estado == EstadoDocumentoElectronico.Procesando).ToListAsync(ct);
        foreach (var d in stale)
        {
            d.Estado = EstadoDocumentoElectronico.Pendiente;
            d.ProximoIntentoEn = DateTime.UtcNow;
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task ProcessOneAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MainDataContext>();
        var factus = scope.ServiceProvider.GetRequiredService<FactusService>();

        // Reclamar un pendiente: cambiar a Procesando con RowVersion.
        var now = DateTime.UtcNow;
        var candidato = await db.DocumentosElectronicos
            .Where(d => d.Estado == EstadoDocumentoElectronico.Pendiente &&
                        (d.ProximoIntentoEn == null || d.ProximoIntentoEn <= now))
            .OrderBy(d => d.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (candidato is null) return;

        // Claim con concurrencia optimista.
        var originalRowVersion = candidato.RowVersion;
        candidato.Estado = EstadoDocumentoElectronico.Procesando;
        candidato.Intentos++;
        try
        {
            db.Entry(candidato).Property(d => d.RowVersion).OriginalValue = originalRowVersion;
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Otro worker lo reclamó.
            return;
        }

        // Procesar.
        try
        {
            await factus.ProcesarDocumentoAsync(candidato.Id, ct);
        }
        catch (FactusValidationException ex)
        {
            candidato.Estado = EstadoDocumentoElectronico.ConErrores;
            candidato.UltimoError = ex.Message;
            candidato.ErroresDian = string.Join("; ", ex.Errors);
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            candidato.Estado = EstadoDocumentoElectronico.Pendiente;
            candidato.UltimoError = ex.Message;
            var delay = TimeSpan.FromSeconds(Math.Min(300, Math.Pow(2, candidato.Intentos)));
            candidato.ProximoIntentoEn = DateTime.UtcNow.Add(delay);
            if (candidato.Intentos >= 5)
            {
                candidato.Estado = EstadoDocumentoElectronico.ConErrores;
                candidato.ProximoIntentoEn = null;
            }
            await db.SaveChangesAsync(ct);
            _logger.LogWarning(ex, "Error procesando documento {Id}, intento {Intento}", candidato.Id, candidato.Intentos);
        }
    }
}
