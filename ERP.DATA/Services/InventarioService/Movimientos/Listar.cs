using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.Movimientos;

public partial class MovimientosManager
{
    public async Task<List<MovimientoDetailDto>> ListMovements(ListMovementsRequest request, CancellationToken cancellationToken = default)
    {
        var query = context.Movements
            .AsNoTracking()
            .AsQueryable();

        if (request.StoreId.HasValue)
        {
            query = query.Where(m => m.OrigenWarehouse.StoreId == request.StoreId);
        }

        if (request.MinDate.HasValue)
            query = query.Where(m => m.CreatedAt >= request.MinDate.Value);

        if (request.MaxDate.HasValue)
            query = query.Where(m => m.CreatedAt <= request.MaxDate.Value);

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

        if (request.PageSize != -1)
        {
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        var movements = await query.ToListAsync(cancellationToken);

        var movementIds = movements.Select(m => m.Id).ToList();

        var details = await context.UnitProductMovements
            .AsNoTracking()
            .Where(upm => movementIds.Contains(upm.MovimientoId))
            .Include(upm => upm.UnidadProducto)
            .Select(upm => new 
            {
                upm.MovimientoId,
                Item = new MovimientoItemDto(
                    upm.UnidadProductoId,
                    upm.UnidadProducto.ProductoVarianteId,
                    upm.UnidadProducto.SerialNumber,
                    upm.UnidadProducto.Lote,
                    upm.UnidadProducto.FechaVencimiento
                )
            })
            .ToListAsync(cancellationToken);

        var detailsGrouped = details
            .GroupBy(d => d.MovimientoId)
            .ToDictionary(g => g.Key, g => g.Select(d => d.Item).ToList());

        var warehouseIds = movements
            .SelectMany(m => new[] { m.OrigenWarehouseId, m.DestinationWarehouseId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var warehouses = await context.Warehouse
            .AsNoTracking()
            .Where(w => warehouseIds.Contains(w.Id))
            .ToDictionaryAsync(w => w.Id, w => w.Name, cancellationToken);

        return movements.Select(m => new MovimientoDetailDto(
            m.Id,
            m.OrigenWarehouseId,
            m.OrigenWarehouseId > 0 && warehouses.TryGetValue(m.OrigenWarehouseId, out var orgName) ? orgName : null,
            m.DestinationWarehouseId,
            m.DestinationWarehouseId.HasValue && m.DestinationWarehouseId.Value > 0 && warehouses.TryGetValue(m.DestinationWarehouseId.Value, out var destName) ? destName : null,
            m.Type,
            m.Quantity,
            m.UnitCost,
            m.TotalCost,
            m.Lote,
            m.FechaVencimiento,
            m.Motive,
            m.Observations,
            detailsGrouped.TryGetValue(m.Id, out var itemsList) ? itemsList : new List<MovimientoItemDto>(),
            m.CreatedAt,
            "Corregir, no existe sesion aun"
        )).ToList();
    }
}