using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
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

            // 3. Resolver costo unitario
            decimal unitCost = (variante.CostoUnitario.HasValue && variante.CostoUnitario.Value > 0) 
                ? variante.CostoUnitario.Value 
                : variante.ProductoBase.CostoUnitario;
            
            // 4. Crear el registro de la cabecera del movimiento en el Kárdex
            var movimiento = new Movement
            {
                OrigenWarehouseId = salida.BodegaId,
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
            await context.SaveChangesAsync(cancellationToken); // Guardar para obtener el Id de la cabecera

            // 5. Manejar el detalle de unidades si el producto maneja inventario físico/serializado
            var unidadesAfectadas = await context.UnidadesProductos
                .Where(u => u.ProductoVarianteId == salida.ProductoVarianteId &&
                            u.BodegaId == salida.BodegaId &&
                            u.Status == UnidadProductoStatus.Available)
                .Take(salida.Cantidad)
                .ToListAsync(cancellationToken);

            var itemsList = new List<MovimientoItemDto>();

            foreach (var unidad in unidadesAfectadas)
            {
                unidad.Status = UnidadProductoStatus.Sold;
                unidad.UpdatedAt = DateTime.UtcNow;
                unidad.UpdatedBy = "systemuser";

                context.UnitProductMovements.Add(new UnitProductMovement
                {
                    UnidadProductoId = unidad.Id,
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = TipoMovimiento.Salida,
                    BodegaOrigenId = salida.BodegaId,
                    BodegaDestinoId = null,
                    Motivo = salida.Motivo ?? "Salida de inventario",
                    Observaciones = salida.Observaciones
                });

                itemsList.Add(new MovimientoItemDto(
                    unidad.Id,
                    unidad.ProductoVarianteId,
                    unidad.SerialNumber,
                    unidad.Lote,
                    unidad.FechaVencimiento
                ));
            }

            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            // Obtener el nombre de la bodega para el DTO
            var nombreBodega = await context.Warehouse
                .Where(w => w.Id == salida.BodegaId)
                .Select(w => w.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            // Retorno adaptado a la nueva estructura con la lista de items
            return new MovimientoDetailDto(
                movimiento.Id,
                salida.BodegaId,
                nombreBodega,
                null,
                null,
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
                itemsList,
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