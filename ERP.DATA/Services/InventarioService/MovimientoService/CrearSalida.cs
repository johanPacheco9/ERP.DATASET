using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;

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
            // 1. Validar la existencia de la variante y obtener su ProductoBase para el costo
            var variante = await context.ProductoVariantes
                .Include(v => v.ProductoBase)
                .FirstOrDefaultAsync(v => v.Id == salida.ProductoVarianteId, cancellationToken)
                ?? throw new InvalidOperationException($"La variante #{salida.ProductoVarianteId} no existe.");

            // 2. Consultar y actualizar el stock acumulado en la bodega
            var stock = await context.WarehouseStock.FirstOrDefaultAsync(
                s => s.WarehouseId == salida.BodegaId &&
                     s.ProductoVarianteId == salida.ProductoVarianteId,
                cancellationToken);

            if (stock == null || stock.CurrentStock < salida.Cantidad)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente en bodega. Disponible: {stock?.CurrentStock ?? 0}, Solicitado: {salida.Cantidad}");
            }

            // Descontar saldo del inventario
            stock.CurrentStock -= salida.Cantidad;
            stock.FechaActualizacion = DateTime.UtcNow;

            // 3. Resolver costo unitario (con fallback a ProductoBase y coalescencia para evitar error CS0266)
            decimal unitCost = (variante.CostoUnitario > 0 
                ? variante.CostoUnitario 
                : variante.ProductoBase.CostoUnitario) ?? 0m;

            // 4. Crear el registro del movimiento de salida en el Kárdex
            var movimiento = new Movement
            {
                WarehouseId = salida.BodegaId,
                ProductoVarianteId = variante.Id,
                Type = TipoMovimiento.Salida,
                Quantity = salida.Cantidad,
                UnitCost = unitCost,
                Lote = salida.Lote,
                Motive = salida.Motivo,
                Observations = salida.Observaciones,
                ReferenceId = salida.ReferenciaId,
                ReferenceType = salida.ReferenciaTipo,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "systemuser"
            };

            context.Movements.Add(movimiento);

            // 5. Guardar cambios y confirmar transacción
            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new MovimientoDetailDto(
                movimiento.Id,
                movimiento.ProductoVarianteId,
                movimiento.UnidadProductoId,
                movimiento.WarehouseId,
                movimiento.Type,
                movimiento.Quantity,
                movimiento.UnitCost,
                movimiento.TotalCost,
                movimiento.ReferenceId,
                movimiento.ReferenceType,
                movimiento.Lote,
                movimiento.FechaVencimiento,
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