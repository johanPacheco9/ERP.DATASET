using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService
{
    public async Task<MovimientoDetailDto?> GetMovementByIdAsync(int movimientoId,
        CancellationToken cancellationToken = default)
    {
        var movimiento = await context.Movements
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == movimientoId, cancellationToken);

        if (movimiento == null) return null;

        // Obtener los items asociados
        var items = await context.UnitProductMovements
            .AsNoTracking()
            .Where(upm => upm.MovimientoId == movimientoId)
            .Include(upm => upm.UnidadProducto)
            .ThenInclude(u => u.ProductoVariante)
            .ThenInclude(v => v.ProductoBase)
            .Select(upm => new MovimientoItemDto(
                upm.UnidadProductoId,
                upm.UnidadProducto.ProductoVarianteId,
                upm.UnidadProducto.SerialNumber,
                upm.UnidadProducto.Lote,
                upm.UnidadProducto.FechaVencimiento
            ))
            .ToListAsync(cancellationToken);

        // Obtener nombres de bodegas
        string? nombreOrigen = null;
        if (movimiento.OrigenWarehouseId > 0)
        {
            nombreOrigen = await context.Warehouse
                .Where(w => w.Id == movimiento.OrigenWarehouseId)
                .Select(w => w.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        string? nombreDestino = null;
        if (movimiento.DestinationWarehouseId.HasValue && movimiento.DestinationWarehouseId.Value > 0)
        {
            nombreDestino = await context.Warehouse
                .Where(w => w.Id == movimiento.DestinationWarehouseId.Value)
                .Select(w => w.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new MovimientoDetailDto(
            movimiento.Id,
            movimiento.OrigenWarehouseId,
            nombreOrigen,
            movimiento.DestinationWarehouseId,
            nombreDestino,
            movimiento.Type,
            movimiento.Quantity,
            movimiento.UnitCost,
            movimiento.TotalCost,
            movimiento.Lote,
            movimiento.FechaVencimiento,
            movimiento.Motive,
            movimiento.Observations,
            items,
            movimiento.CreatedAt,
            "Corregir, no existe sesion aun"
        );
    }
}