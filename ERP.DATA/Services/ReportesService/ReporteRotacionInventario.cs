using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.API.Reportes.Requests;
using ERP.TRAN.CrossLayers.API.Reportes.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.ReportesService;

public partial class ReportesService
{
    /// <summary>
    /// Genera el reporte de rotación y valorización de inventario, combinando tres fuentes de datos:
    /// (1) las "capas" FIFO activas (entradas de Movement con remanente sin consumir),
    /// (2) el stock agregado actual por bodega (WarehouseStock), y
    /// (3) las ventas/consumos ocurridos en el período solicitado.
    /// 
    /// El objetivo es responder, por cada variante de producto en cada bodega:
    /// cuánto valor de inventario hay inmovilizado (a costo FIFO real, no a un costo genérico),
    /// qué tan rápido está rotando ese inventario, y si conviene reordenarlo, mantenerlo o liquidarlo.
    /// </summary>
    /// <param name="request">
    /// Filtros del reporte: rango de fechas (Desde/Hasta, por defecto últimos 30 días),
    /// bodega, sucursal (BranchId), categoría, texto de búsqueda libre y clasificación de rotación a filtrar.
    /// </param>
    /// <param name="cancellationToken">Token de cancelación estándar.</param>
    /// <returns>
    /// Un <see cref="Result{T}"/> con el DTO completo del reporte (items por variante/bodega,
    /// capas FIFO detalladas, agrupaciones por bodega/categoría y totales globales),
    /// o un <see cref="Result{T}"/> fallido si ocurre un error inesperado durante el cálculo.
    /// </returns>
    public async Task<Result<ReporteRotacionInventarioDto>> ObtenerReporteRotacionInventarioAsync(
        ReporteRotacionInventarioRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fechaHasta = request.Hasta ?? DateTime.UtcNow;
            var fechaDesde = request.Desde ?? fechaHasta.AddDays(-30);

            var inicioUtc = DateTime.SpecifyKind(fechaDesde, DateTimeKind.Utc);
            var finUtc = DateTime.SpecifyKind(fechaHasta, DateTimeKind.Utc);
            var totalDias = Math.Max(1, (int)(finUtc.Date - inicioUtc.Date).TotalDays + 1);

            // ============================================================
            // 1. CAPAS FIFO ACTIVAS
            // ============================================================
            // Cada Movement de tipo Entrada representa un "lote" de mercancía que ingresó
            // a un costo específico (UnitCost). RemainingQuantity indica cuánto de ese lote
            // aún no se ha consumido por ventas u otras salidas.
            // Solo nos interesan las capas con remanente > 0: son las que todavía "valen"
            // como inventario físico disponible, y las que StockHelper.DeductInventoryAsync
            // consumiría en orden cronológico (más antigua primero) en la próxima venta.
            var fifoQuery = _context.Movements
                .AsNoTracking()
                .Where(m => m.Type == TipoMovimiento.Entrada && m.RemainingQuantity.HasValue && m.RemainingQuantity.Value > 0);

            if (request.WarehouseId.HasValue && request.WarehouseId.Value > 0)
            {
                fifoQuery = fifoQuery.Where(m => m.OrigenWarehouseId == request.WarehouseId.Value);
            }

            if (request.BranchId.HasValue && request.BranchId.Value > 0)
            {
                fifoQuery = fifoQuery.Where(m => m.OrigenWarehouse.StoreId == request.BranchId.Value);
            }

            // OrderBy(CreatedAt): mantenemos el orden FIFO real (más antiguo primero) también
            // en el reporte, para que las capas se lean en el mismo orden en que se consumirían.
            var capasFifoDb = await fifoQuery
                .Include(m => m.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
                .Include(m => m.OrigenWarehouse)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);

            var capasFifoDto = capasFifoDb.Select(c => new CapaFifoDisponibleDto(
                c.Id,
                c.ProductoVarianteId,
                c.ProductoVariante.SKU,
                c.ProductoVariante.ProductoBase?.Name ?? "Producto",
                c.OrigenWarehouseId,
                c.OrigenWarehouse?.Name ?? "Bodega",
                c.CreatedAt,
                c.Quantity,
                c.RemainingQuantity ?? 0,
                c.UnitCost,
                (c.RemainingQuantity ?? 0) * c.UnitCost, // Valor de la capa: lo que queda por vender de ese lote, a SU costo real
                c.Lote,
                c.FechaVencimiento
            )).ToList();

            // ============================================================
            // 2. SALDOS DE STOCK EN BODEGAS
            // ============================================================
            // WarehouseStock.CurrentStock es el contador agregado de unidades disponibles
            // por variante/bodega, independiente de a qué costo entraron. Es la fuente de
            // "cuántas unidades hay" (para reservas, ventas, etc.), mientras que las capas
            // FIFO de arriba son la fuente de "a qué costo entraron esas unidades".
            //
            // FIX: se agrega AsSplitQuery() porque el Include de ProductoBase.Categorias
            // es una colección (relación muchos a muchos). Sin split, EF genera un único
            // JOIN que duplica cada fila de WarehouseStock una vez por cada categoría
            // asociada al producto (explosión cartesiana) — esto inflaba el stock total
            // reportado muy por encima del valor real en base de datos. AsSplitQuery()
            // ejecuta consultas separadas por colección y las combina en memoria sin
            // duplicar la fila padre.
            var stockQuery = _context.WarehouseStock
                .AsNoTracking()
                .Include(ws => ws.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
                        .ThenInclude(pb => pb.Categorias)
                            .ThenInclude(c => c.Category)
                .Include(ws => ws.Warehouse)
                .AsSplitQuery()
                .AsQueryable();

            if (request.WarehouseId.HasValue && request.WarehouseId.Value > 0)
            {
                stockQuery = stockQuery.Where(ws => ws.WarehouseId == request.WarehouseId.Value);
            }

            if (request.BranchId.HasValue && request.BranchId.Value > 0)
            {
                stockQuery = stockQuery.Where(ws => ws.Warehouse.StoreId == request.BranchId.Value);
            }

            if (request.CategoriaId.HasValue && request.CategoriaId.Value > 0)
            {
                stockQuery = stockQuery.Where(ws => ws.ProductoVariante.ProductoBase.Categorias.Any(c => c.CategoryId == request.CategoriaId.Value));
            }

            var stocks = await stockQuery.ToListAsync(cancellationToken);

            // ============================================================
            // 3. VENTAS Y CONSUMOS EN EL PERIODO
            // ============================================================
            // Se usa CostoVentaTotal (ya calculado y guardado en cada SaleLineItem al momento
            // de la venta, ver StockHelper.DeductInventoryAsync) en vez de recalcular el costo
            // FIFO aquí: eso garantiza que el reporte refleje el costo REAL que se aplicó en
            // cada venta histórica, no una aproximación nueva basada en el estado actual de las capas.
            var salesLineQuery = _context.SaleLineItems
                .AsNoTracking()
                .Where(l => l.Sale.Status == SaleStatus.Completed &&
                            l.Sale.CreatedAt >= inicioUtc &&
                            l.Sale.CreatedAt <= finUtc);

            if (request.WarehouseId.HasValue && request.WarehouseId.Value > 0)
            {
                salesLineQuery = salesLineQuery.Where(l => l.Sale.WarehouseId == request.WarehouseId.Value);
            }

            if (request.BranchId.HasValue && request.BranchId.Value > 0)
            {
                salesLineQuery = salesLineQuery.Where(l => l.Sale.StoreId == request.BranchId.Value);
            }

            var saleLines = await salesLineQuery
                .Select(l => new
                {
                    l.ProductoVarianteId,
                    WarehouseId = l.Sale.WarehouseId,
                    l.Quantity,
                    l.UnitPrice,
                    l.CostoVentaTotal,
                    l.Sale.CreatedAt
                })
                .ToListAsync(cancellationToken);

            // Se agrupa por (Variante, Bodega) para poder cruzar cada línea de venta
            // contra su stock y sus capas FIFO correspondientes en el paso 4.
            var saleLinesDict = saleLines
                .GroupBy(l => (l.ProductoVarianteId, l.WarehouseId))
                .ToDictionary(
                    g => (g.Key.ProductoVarianteId, g.Key.WarehouseId),
                    g => new
                    {
                        UnidadesVendidas = g.Sum(x => x.Quantity),
                        Ingresos = g.Sum(x => x.UnitPrice * x.Quantity),
                        CostoVenta = g.Sum(x => x.CostoVentaTotal),
                        UltimaVenta = g.Max(x => (DateTime?)x.CreatedAt)
                    }
                );

            // Mismo agrupamiento para las capas FIFO: permite saber, por variante/bodega,
            // cuánto valor total de inventario queda y cuándo entró la capa más reciente.
            var capasDict = capasFifoDb
                .GroupBy(c => (c.ProductoVarianteId, c.OrigenWarehouseId))
                .ToDictionary(
                    g => (g.Key.ProductoVarianteId, g.Key.OrigenWarehouseId),
                    g => new
                    {
                        TotalValorFifo = g.Sum(c => (c.RemainingQuantity ?? 0) * c.UnitCost),
                        TotalRemanente = g.Sum(c => c.RemainingQuantity ?? 0),
                        UltimaEntrada = g.Max(c => (DateTime?)c.CreatedAt)
                    }
                );

            // ============================================================
            // 4. MAPEO DE ITEMS (una fila por variante/bodega)
            // ============================================================
            var items = new List<ItemRotacionInventarioDto>();

            foreach (var stock in stocks)
            {
                var key = (stock.ProductoVarianteId, stock.WarehouseId);
                capasDict.TryGetValue(key, out var capasInfo);
                saleLinesDict.TryGetValue(key, out var salesInfo);

                var valorFifo = capasInfo?.TotalValorFifo ?? 0m;

                // Costo promedio "implícito": el valor total de las capas FIFO restantes
                // dividido entre las unidades físicas en stock. Sirve como referencia de
                // costo unitario cuando no interesa desglosar por capa individual.
                var costoPromedioFifo = stock.CurrentStock > 0 && valorFifo > 0
                    ? Math.Round(valorFifo / stock.CurrentStock, 4)
                    : 0m;

                var unidadesVendidas = salesInfo?.UnidadesVendidas ?? 0;
                var costoVentasPeriodo = salesInfo?.CostoVenta ?? 0m;
                var totalIngresosPeriodo = salesInfo?.Ingresos ?? 0m;

                // ------------------------------------------------------
                // Índice de rotación: cuántas veces "se dio vuelta" el inventario
                // valorizado durante el período. Se prioriza el cálculo por VALOR
                // (costo vendido / valor de inventario) porque es más preciso que
                // contar unidades cuando hay productos de distinto costo unitario.
                // Solo se cae a rotación por UNIDADES si no hay capas FIFO con valor
                // (ej. inventario que nunca tuvo una Entrada correctamente costeada).
                // ------------------------------------------------------
                decimal indiceRotacion = 0m;
                if (valorFifo > 0)
                {
                    indiceRotacion = Math.Round(costoVentasPeriodo / valorFifo, 2);
                }
                else if (stock.CurrentStock > 0)
                {
                    indiceRotacion = Math.Round((decimal)unidadesVendidas / stock.CurrentStock, 2);
                }
                else if (unidadesVendidas > 0)
                {
                    // Se vendió todo lo que había en stock (quedó en 0) y no hay capas FIFO:
                    // se marca como rotación muy alta en vez de indefinida/infinita.
                    indiceRotacion = 99m; // Alta rotación
                }

                // Días de inventario: cuánto tiempo, en promedio, tarda en agotarse el stock actual
                // al ritmo de ventas del período. Es la inversa del índice de rotación, escalada
                // a la cantidad real de días que cubre el reporte.
                decimal diasInventario = 0m;
                if (indiceRotacion > 0)
                {
                    diasInventario = Math.Round(totalDias / indiceRotacion, 1);
                }

                // ------------------------------------------------------
                // Clasificación cualitativa de rotación, para que el cliente pueda
                // filtrar/priorizar sin tener que interpretar los números crudos:
                // - "Sin Movimiento": hay stock pero no se vendió nada en el período (alerta de producto estancado).
                // - "Alta": rotación >= 2x en el período, o se agota en 30 días o menos.
                // - "Media": rotación >= 1x, o se agota en 60 días o menos.
                // - "Baja": todo lo demás (rota lento, capital inmovilizado por más tiempo).
                // ------------------------------------------------------
                string clasificacion;
                if (unidadesVendidas == 0 && stock.CurrentStock > 0)
                {
                    clasificacion = "Sin Movimiento";
                }
                else if (indiceRotacion >= 2.0m || (diasInventario > 0 && diasInventario <= 30))
                {
                    clasificacion = "Alta";
                }
                else if (indiceRotacion >= 1.0m || (diasInventario > 0 && diasInventario <= 60))
                {
                    clasificacion = "Media";
                }
                else
                {
                    clasificacion = "Baja";
                }

                // Se toma solo la primera categoría asociada al producto para mostrar en el
                // reporte (un producto puede tener varias, pero aquí se simplifica a una).
                var catName = stock.ProductoVariante?.ProductoBase?.Categorias?.FirstOrDefault()?.Category?.Name;

                var item = new ItemRotacionInventarioDto(
                    stock.ProductoVarianteId,
                    stock.ProductoVariante?.SKU ?? "SKU",
                    stock.ProductoVariante?.CodigoBarras,
                    stock.ProductoVariante?.ProductoBase?.Name ?? "Sin nombre",
                    catName,
                    stock.WarehouseId,
                    stock.Warehouse?.Name ?? "Bodega",
                    stock.CurrentStock,
                    stock.StockReservado,
                    stock.StockMinimo,
                    costoPromedioFifo,
                    valorFifo,
                    unidadesVendidas,
                    costoVentasPeriodo,
                    totalIngresosPeriodo,
                    indiceRotacion,
                    diasInventario,
                    clasificacion,
                    capasInfo?.UltimaEntrada,
                    salesInfo?.UltimaVenta
                );

                items.Add(item);
            }

            // Filtros que se aplican en memoria porque dependen de valores YA CALCULADOS
            // (clasificación, texto compuesto de varios campos) y no se pueden traducir
            // limpiamente a SQL en la query original.
            if (!string.IsNullOrWhiteSpace(request.Clasificacion) && request.Clasificacion != "Todos")
            {
                items = items.Where(i => string.Equals(i.ClasificacionRotacion, request.Clasificacion, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim().ToLower();
                items = items.Where(i =>
                    i.NombreProducto.ToLower().Contains(term) ||
                    i.Sku.ToLower().Contains(term) ||
                    (i.CodigoBarras != null && i.CodigoBarras.ToLower().Contains(term)) ||
                    (i.Categoria != null && i.Categoria.ToLower().Contains(term))
                ).ToList();
            }

            // ============================================================
            // 5. AGRUPACIONES DE VALORIZACIÓN (para vistas resumen/gráficos)
            // ============================================================
            var valorizacionBodega = items
                .GroupBy(i => new { i.BodegaId, i.NombreBodega })
                .Select(g => new ValorizacionPorBodegaDto(
                    g.Key.BodegaId,
                    g.Key.NombreBodega,
                    g.Sum(x => x.StockActual),
                    g.Sum(x => x.ValorTotalStockFifo),
                    g.Select(x => x.ProductoVarianteId).Distinct().Count()
                ))
                .ToList();

            var valorizacionCategoria = items
                .GroupBy(i => i.Categoria ?? "Sin Categoría")
                .Select(g => new ValorizacionPorCategoriaDto(
                    0, // Id de categoría no disponible tras el agrupamiento por nombre; ver TODO abajo
                    g.Key,
                    g.Sum(x => x.StockActual),
                    g.Sum(x => x.ValorTotalStockFifo)
                ))
                .ToList();

            // ============================================================
            // 6. TOTALES GLOBALES DEL REPORTE
            // ============================================================
            var totalUnidadesEnStock = items.Sum(i => i.StockActual);
            var totalValorInventarioFifo = items.Sum(i => i.ValorTotalStockFifo);
            var totalCostoVendidoPeriodo = items.Sum(i => i.CostoVentasPeriodo);

            // Mismo criterio de rotación que a nivel de item, pero aplicado al total
            // del inventario filtrado, para dar una cifra única de salud general.
            var indiceRotacionGlobal = totalValorInventarioFifo > 0
                ? Math.Round(totalCostoVendidoPeriodo / totalValorInventarioFifo, 2)
                : 0m;

            var diasInventarioGlobal = indiceRotacionGlobal > 0
                ? Math.Round(totalDias / indiceRotacionGlobal, 1)
                : 0m;

            var cantAlta = items.Count(i => i.ClasificacionRotacion == "Alta");
            var cantMedia = items.Count(i => i.ClasificacionRotacion == "Media");
            var cantBaja = items.Count(i => i.ClasificacionRotacion == "Baja");
            var cantSinMov = items.Count(i => i.ClasificacionRotacion == "Sin Movimiento");

            var dto = new ReporteRotacionInventarioDto(
                inicioUtc,
                finUtc,
                totalDias,
                items.Count,
                totalUnidadesEnStock,
                totalValorInventarioFifo,
                totalCostoVendidoPeriodo,
                indiceRotacionGlobal,
                diasInventarioGlobal,
                cantAlta,
                cantMedia,
                cantBaja,
                cantSinMov,
                items.OrderByDescending(i => i.ValorTotalStockFifo).ToList(), // Los de mayor valor inmovilizado primero
                capasFifoDto,
                valorizacionBodega,
                valorizacionCategoria
            );

            return Result<ReporteRotacionInventarioDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de rotación y valoración de inventario FIFO.");
            return Result<ReporteRotacionInventarioDto>.Failure(Error.Failure("Reporte.RotacionError", ex.Message));
        }
    }
}