using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService
{
    public async Task<List<MovimientoDetailDto>> ListMovements(ListMovementsRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Iniciamos la consulta base (Asegúrate de que tu DbSet de movimientos se llame Movements o Movimientos)
        var query = context.Movements
            .AsNoTracking()
            .Include(m => m.Warehouse) // Necesario si filtras por StoreId a través de la bodega
            .AsQueryable();

        // 2. Aplicamos filtros opcionales (StoreId y Fechas)
        if (request.StoreId.HasValue)
        {
            query = query.Where(m => m.Warehouse.StoreId == request.StoreId);
        }

        if (request.MinDate.HasValue)
            query = query.Where(m => m.CreatedAt >= request.MinDate.Value);

        if (request.MaxDate.HasValue)
            query = query.Where(m => m.CreatedAt <= request.MaxDate.Value);

        // 3. Ordenamiento dinámico seguro
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = request.OrderBy.Contains("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(m => m.Id)
                : query.OrderBy(m => m.Id);
        }
        else
        {
            query = query.OrderByDescending(m => m.CreatedAt);
        }

        // 4. Proyección exacta al DTO (Record) alineada con ProductoVariante y UnidadProducto
        var projectedQuery = query.Select(m => new MovimientoDetailDto(
            m.Id,                        // MovimientoId
            m.ProductoVarianteId,        // ProductoVarianteId (Alineado al nuevo modelo)
            m.UnidadProductoId,          // UnidadProductoId (O null si es un movimiento por lote/granel)
            m.WarehouseId,               // BodegaId
            m.Type,                      // TipoMovimiento
            m.Quantity,                  // Cantidad
            m.UnitCost,                  // CostoUnitario
            m.TotalCost,                 // CostoTotal
            m.ReferenceId,               // ReferenciaId (ID de la venta, compra o traslado relacionado)
            m.ReferenceType,             // ReferenciaTipo
            m.Lote,                      // Lote
            m.FechaVencimiento,          // FechaVencimiento
            m.Motive,                    // Motivo
            m.Observations,              // Observaciones
            m.CreatedAt,                 // CreatedAt
            m.CreatedBy                  // CreatedBy
        ));

        // 5. Paginación
        if (request.PageSize != -1)
        {
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;

            projectedQuery = projectedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        return await projectedQuery.ToListAsync(cancellationToken);
    }
}