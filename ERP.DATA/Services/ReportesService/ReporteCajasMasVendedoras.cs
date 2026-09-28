using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.API.Reportes.Requests;
using ERP.TRAN.CrossLayers.API.Reportes.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.ReportesService;

public partial class ReportesService
{
    public async Task<Result<ReporteCajasMasVendedorasDto>> ObtenerReporteCajasMasVendedorasAsync(
        ReporteCajasMasVendedorasRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Sales
                .AsNoTracking()
                .Where(s => s.Status == SaleStatus.Completed && s.PosTerminalId.HasValue);

            if (request.FechaInicio.HasValue)
            {
                var inicioUtc = DateTime.SpecifyKind(request.FechaInicio.Value, DateTimeKind.Utc);
                query = query.Where(s => s.CreatedAt >= inicioUtc);
            }

            if (request.FechaFin.HasValue)
            {
                var finUtc = DateTime.SpecifyKind(request.FechaFin.Value, DateTimeKind.Utc);
                query = query.Where(s => s.CreatedAt <= finUtc);
            }

            if (request.StoreId.HasValue && request.StoreId.Value > 0)
            {
                query = query.Where(s => s.StoreId == request.StoreId.Value);
            }

            if (request.PosTerminalId.HasValue && request.PosTerminalId.Value > 0)
            {
                query = query.Where(s => s.PosTerminalId == request.PosTerminalId.Value);
            }

            var sales = await query
                .Include(s => s.PosTerminal)
                .Include(s => s.Store)
                .Include(s => s.Payments)
                .ToListAsync(cancellationToken);

            var cajasAgrupadas = sales
                .GroupBy(s => new
                {
                    TerminalId = s.PosTerminalId!.Value,
                    TerminalNombre = s.PosTerminal != null ? s.PosTerminal.Name : $"Terminal #{s.PosTerminalId}",
                    TerminalCode = s.PosTerminal != null ? s.PosTerminal.Code : $"POS-{s.PosTerminalId}",
                    StoreNombre = s.Store != null ? s.Store.Name : $"Tienda #{s.StoreId}"
                })
                .Select(g =>
                {
                    var totalVendido = g.Sum(s => s.Total);
                    var count = g.Count();
                    var ticketPromedio = count > 0 ? Math.Round(totalVendido / count, 2) : 0m;

                    var pagos = g.SelectMany(s => s.Payments).ToList();
                    var efectivo = pagos.Where(p => p.Method == PaymentMethod.Cash).Sum(p => p.Amount);
                    var tarjeta = pagos.Where(p => p.Method is PaymentMethod.DebitCard or PaymentMethod.CreditCard).Sum(p => p.Amount);
                    var transf = pagos.Where(p => p.Method is PaymentMethod.BankTransfer or PaymentMethod.Nequi).Sum(p => p.Amount);
                    var credito = pagos.Where(p => p.Method == PaymentMethod.Credit).Sum(p => p.Amount);

                    return new CajaMasVendedoraItemDto(
                        g.Key.TerminalId,
                        g.Key.TerminalNombre,
                        g.Key.TerminalCode,
                        g.Key.StoreNombre,
                        count,
                        totalVendido,
                        efectivo,
                        tarjeta,
                        transf,
                        credito,
                        ticketPromedio
                    );
                })
                .OrderByDescending(c => c.TotalVendido)
                .ToList();

            var dto = new ReporteCajasMasVendedorasDto(
                cajasAgrupadas.Sum(c => c.TotalVendido),
                cajasAgrupadas.Sum(c => c.TotalTransacciones),
                cajasAgrupadas
            );

            return Result<ReporteCajasMasVendedorasDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar Reporte de Cajas Más Vendedoras");
            return Result<ReporteCajasMasVendedorasDto>.Failure(Error.Failure("Reporte.TopCajasError", ex.Message));
        }
    }
}
