using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.API.Reportes.Requests;
using ERP.TRAN.CrossLayers.API.Reportes.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.ReportesService;

public partial class ReportesService
{
    public async Task<Result<ReporteUtilidadDto>> ObtenerReporteUtilidadAsync(
        ReporteUtilidadRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Sales
                .AsNoTracking()
                .Where(s => s.Status == SaleStatus.Completed);

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

            if (request.WarehouseId.HasValue && request.WarehouseId.Value > 0)
            {
                query = query.Where(s => s.WarehouseId == request.WarehouseId.Value);
            }

            var sales = await query
                .Include(s => s.Lines)
                    .ThenInclude(l => l.ProductoVariante)
                        .ThenInclude(v => v.ProductoBase)
                .ToListAsync(cancellationToken);

            var totalIngresos = sales.Sum(s => s.Subtotal);
            var totalImpuestos = sales.Sum(s => s.TaxAmount);
            var totalCosto = sales.SelectMany(s => s.Lines).Sum(l => l.CostoVentaTotal);
            var utilidadBruta = totalIngresos - totalCosto;
            var margenPorcentaje = totalIngresos > 0 
                ? Math.Round((utilidadBruta / totalIngresos) * 100m, 2) 
                : 0m;
            var totalUnidades = sales.SelectMany(s => s.Lines).Sum(l => l.Quantity);

            var desglose = sales
                .SelectMany(s => s.Lines)
                .GroupBy(l => new 
                {
                    l.ProductoVarianteId,
                    ProductoBaseId = l.ProductoVariante.ProductoBaseId,
                    Nombre = l.ProductoVariante.ProductoBase.Name,
                    Sku = l.ProductoVariante.SKU
                })
                .Select(g =>
                {
                    var ingresoItem = g.Sum(x => x.UnitPrice * x.Quantity);
                    var costoItem = g.Sum(x => x.CostoVentaTotal);
                    var utilidadItem = ingresoItem - costoItem;
                    var margenItem = ingresoItem > 0 
                        ? Math.Round((utilidadItem / ingresoItem) * 100m, 2) 
                        : 0m;

                    return new ItemUtilidadProductoDto(
                        g.Key.ProductoBaseId,
                        g.Key.ProductoVarianteId,
                        g.Key.Nombre,
                        g.Key.Sku,
                        g.Sum(x => x.Quantity),
                        ingresoItem,
                        costoItem,
                        utilidadItem,
                        margenItem
                    );
                })
                .OrderByDescending(d => d.UtilidadBruta)
                .ToList();

            var dto = new ReporteUtilidadDto(
                totalIngresos,
                totalCosto,
                utilidadBruta,
                margenPorcentaje,
                totalImpuestos,
                sales.Count,
                totalUnidades,
                desglose
            );

            return Result<ReporteUtilidadDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar Reporte de Utilidad");
            return Result<ReporteUtilidadDto>.Failure(Error.Failure("Reporte.UtilidadError", ex.Message));
        }
    }
}
