using System.Security.Claims;
using ERP.DATA.Repositories;
using ERP.DATA.Services.FactusService;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATASET.Api;

public static class FactusApiExtensions
{
    public static WebApplication MapFactusApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/pos/ventas/{saleId:int}/factus")
            .RequireAuthorization();

        // GET documentos electrónicos de la venta
        group.MapGet("/documentos", async (int saleId, MainDataContext db, CancellationToken ct) =>
        {
            var docs = await db.DocumentosElectronicos
                .AsNoTracking()
                .Where(d => d.VentaId == saleId)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new { d.Id, d.Tipo, d.Estado, d.Numero, d.IsValidated, d.ValidatedAt, d.QrUrl, d.PublicUrl })
                .ToListAsync(ct);
            return Results.Ok(docs);
        });

        // GET pdf
        group.MapGet("/{documentoId:int}/pdf", async (int saleId, int documentoId, ClaimsPrincipal user, FactusService factus, MainDataContext db, CancellationToken ct) =>
        {
            if (!await TieneAccesoAsync(saleId, db, ct)) return Results.NotFound();
            var doc = await db.DocumentosElectronicos.AsNoTracking().FirstOrDefaultAsync(d => d.Id == documentoId && d.VentaId == saleId, ct);
            if (doc is null) return Results.NotFound();

            var res = await factus.DescargarPdfAsync(documentoId, ct);
            if (!res.IsSuccess) return Results.BadRequest(new { message = res.Error });
            return Results.File(res.Bytes!, res.ContentType!, res.FileName!);
        });

        // GET xml
        group.MapGet("/{documentoId:int}/xml", async (int saleId, int documentoId, ClaimsPrincipal user, FactusService factus, MainDataContext db, CancellationToken ct) =>
        {
            if (!await TieneAccesoAsync(saleId, db, ct)) return Results.NotFound();
            var doc = await db.DocumentosElectronicos.AsNoTracking().FirstOrDefaultAsync(d => d.Id == documentoId && d.VentaId == saleId, ct);
            if (doc is null) return Results.NotFound();

            var res = await factus.DescargarXmlAsync(documentoId, ct);
            if (!res.IsSuccess) return Results.BadRequest(new { message = res.Error });
            return Results.File(res.Bytes!, res.ContentType!, res.FileName!);
        });

        return app;
    }

    private static async Task<bool> TieneAccesoAsync(int saleId, MainDataContext db, CancellationToken ct)
    {
        return await db.Sales.AnyAsync(s => s.Id == saleId, ct);
    }
}
