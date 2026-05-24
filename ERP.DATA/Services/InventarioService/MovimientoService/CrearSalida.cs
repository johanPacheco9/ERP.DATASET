using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using Microsoft.EntityFrameworkCore;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService
{
    public async Task<MovimientoDetailDto> RegistrarSalidaAsync(
     CreateExitMovementRequest salida,
     CancellationToken cancellationToken)
    {
        if (!salida.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {

            var lineaProductoId = salida.LineaProductoId > 0
                ? salida.LineaProductoId
                : salida.ProductoVarianteId;

            var unidadesDisponibles = await context.Productos
                .Where(u =>
                    u.LineaProductoId == lineaProductoId &&
                    u.BodegaId == salida.BodegaId &&
                    u.Status == ProductoStatus.Available)
                .OrderBy(u => u.CreatedAt)
                .ThenBy(u => u.Id)
                .Take(salida.Cantidad)
                .ToListAsync(cancellationToken);

            if (unidadesDisponibles.Count < salida.Cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente. Disponible: {unidadesDisponibles.Count}");

            // 2. Crear movimiento agregado
            var movimiento = new Movement
            {
                WarehouseId = salida.BodegaId,
                LineaProductoId = lineaProductoId,
                ProductId = unidadesDisponibles.First().Id,
                Type = TipoMovimiento.Salida,
                Quantity = salida.Cantidad,
                UnitCost = 0m, // opcional: promedio / FIFO
                Motive = salida.Motivo,
                Observations = salida.Observaciones,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "systemuser"
            };

            context.Movements.Add(movimiento);
            await context.SaveChangesAsync(cancellationToken);

            foreach (var unidad in unidadesDisponibles)
            {
                unidad.Status = ProductoStatus.Sold;
                unidad.UpdatedAt = DateTime.UtcNow;

                context.UnitProductMovements.Add(new UnitProductMovement
                {
                    ProductoId = unidad.Id,
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = TipoMovimiento.Salida,
                    BodegaOrigenId = salida.BodegaId,
                    Motivo = salida.Motivo,
                    Observaciones = $"Salida por movimiento #{movimiento.Id}"
                });
            }

            // 4. Actualizar stock agregado
            var stock = await context.WarehouseStock.FirstOrDefaultAsync(
                s => s.WarehouseId == salida.BodegaId &&
                     s.LineaProductoId == lineaProductoId,
                cancellationToken);

            if (stock != null)
            {
                stock.CurrentStock -= salida.Cantidad;
                stock.FechaActualizacion = DateTime.UtcNow;
            }

            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new MovimientoDetailDto(
                movimiento.Id,
                movimiento.ProductId!.Value,
                movimiento.LineaProductoId,
                movimiento.WarehouseId,
                movimiento.Type,
                movimiento.Quantity,
                movimiento.UnitCost,
                movimiento.TotalCost,
                movimiento.Lote,
                movimiento.ReferenceTye,
                movimiento.Motive,
                movimiento.Observations,
                movimiento.CreatedAt,
                movimiento.CreatedBy
            );
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
