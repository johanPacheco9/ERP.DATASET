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
    public async Task<Result<ReporteProductosMasVendidosDto>> ObtenerReporteProductosMasVendidosAsync(
        ReporteProductosMasVendidosRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.SaleLineItems
                .AsNoTracking()
                .Where(l => l.Sale.Status == SaleStatus.Completed);

            if (request.FechaInicio.HasValue)
            {
                var inicioUtc = DateTime.SpecifyKind(request.FechaInicio.Value, DateTimeKind.Utc);
                query = query.Where(l => l.Sale.CreatedAt >= inicioUtc);
            }

            if (request.FechaFin.HasValue)
            {
                var finUtc = DateTime.SpecifyKind(request.FechaFin.Value, DateTimeKind.Utc);
                query = query.Where(l => l.Sale.CreatedAt <= finUtc);
            }

            if (request.StoreId.HasValue && request.StoreId.Value > 0)
            {
                query = query.Where(l => l.Sale.StoreId == request.StoreId.Value);
            }

            if (request.WarehouseId.HasValue && request.WarehouseId.Value > 0)
            {
                query = query.Where(l => l.Sale.WarehouseId == request.WarehouseId.Value);
            }

            int topCount = request.Top > 0 ? request.Top : 10;

            var queryAgrupado = query
                .Include(l => l.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
                .GroupBy(l => new
                {
                    l.ProductoVarianteId,
                    ProductoBaseId = l.ProductoVariante.ProductoBaseId,
                    Nombre = l.ProductoVariante.ProductoBase.Name,
                    Sku = l.ProductoVariante.SKU
                })
                .Select(g => new
                {
                    g.Key.ProductoBaseId,
                    g.Key.ProductoVarianteId,
                    g.Key.Nombre,
                    g.Key.Sku,
                    CantidadVendida = g.Sum(x => x.Quantity),
                    TotalVendido = g.Sum(x => x.LineTotal),
                    CostoTotal = g.Sum(x => x.CostoVentaTotal)
                });

            var agrupado = request.OrdenarPorTotal
                ? await queryAgrupado.OrderByDescending(x => x.TotalVendido).Take(topCount).ToListAsync(cancellationToken)
                : await queryAgrupado.OrderByDescending(x => x.CantidadVendida).Take(topCount).ToListAsync(cancellationToken);

            int ranking = 1;
            var productosDto = agrupado.Select(item =>
            {
                var utilidad = item.TotalVendido - item.CostoTotal;
                var margen = item.TotalVendido > 0 
                    ? Math.Round((utilidad / item.TotalVendido) * 100m, 2) 
                    : 0m;

                return new ProductoMasVendidoItemDto(
                    ranking++,
                    item.ProductoBaseId,
                    item.ProductoVarianteId,
                    item.Nombre,
                    item.Sku,
                    item.CantidadVendida,
                    item.TotalVendido,
                    item.CostoTotal,
                    utilidad,
                    margen
                );
            }).ToList();

            var dto = new ReporteProductosMasVendidosDto(
                productosDto.Count,
                productosDto.Sum(p => p.CantidadVendida),
                productosDto.Sum(p => p.TotalVendido),
                productosDto
            );

            return Result<ReporteProductosMasVendidosDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar Reporte de Productos Más Vendidos");
            return Result<ReporteProductosMasVendidosDto>.Failure(Error.Failure("Reporte.TopProductosError", ex.Message));
        }
    }
}
