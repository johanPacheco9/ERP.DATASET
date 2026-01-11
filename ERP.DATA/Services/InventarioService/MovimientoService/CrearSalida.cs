using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService
{
    public async Task<MovimientoDetailDto> RegistrarSalidaAsync(
     CreateExitMovementRequest salida,
     CancellationToken cancellationToken)
    {
        if (!salida.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {

            var unidadesDisponibles = await _context.UnitProduct
                .Where(u =>
                    u.ProductoVarianteId == salida.ProductoVarianteId &&
                    u.BodegaId == salida.BodegaId &&
                    u.UnitProductStatus == UnitProductStatus.Available)
                .OrderBy(u => u.FechaIngreso) // FIFO REAL
                .ThenBy(u => u.Id)
                .Take(salida.Cantidad)
                .ToListAsync(cancellationToken);

            if (unidadesDisponibles.Count < salida.Cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente. Disponible: {unidadesDisponibles.Count}");

            // 2. Crear movimiento agregado
            var movimiento = new Movimiento
            {
                BodegaId = salida.BodegaId,
                ProductoVarianteId = salida.ProductoVarianteId,
                ProductoId = unidadesDisponibles.First().ProductoId,
                TipoMovimiento = TipoMovimiento.Salida,
                Cantidad = salida.Cantidad,
                CostoUnitario = 0m, // opcional: promedio / FIFO
                Motivo = salida.Motivo,
                Observaciones = salida.Observaciones,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "systemuser"
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var unidad in unidadesDisponibles)
            {
                unidad.UnitProductStatus = UnitProductStatus.Sold;
                unidad.UpdatedAt = DateTime.UtcNow;

                _context.UnitProductMovements.Add(new UnitProductMovement
                {
                    ProductoUnidadId = unidad.Id,
                    TipoMovimiento = TipoMovimiento.Salida,
                    BodegaOrigenId = salida.BodegaId,
                    BodegaDestinoId = null,
                    Motivo = salida.Motivo,
                    Observaciones = $"Salida por movimiento #{movimiento.Id}"
                });
            }

            // 4. Actualizar stock agregado
            var stock = await _context.StockBodegas.FirstAsync(
                s => s.BodegaId == salida.BodegaId &&
                     s.ProductoVarianteId == salida.ProductoVarianteId,
                cancellationToken);

            stock.StockActual -= salida.Cantidad;
            stock.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new MovimientoDetailDto(
                movimiento.Id,
                movimiento.ProductoId!.Value,
                movimiento.ProductoVarianteId,
                movimiento.BodegaId,
                movimiento.TipoMovimiento,
                movimiento.Cantidad,
                movimiento.CostoUnitario,
                movimiento.CostoTotal,
                movimiento.Lote,
                movimiento.ReferenciaTipo,
                movimiento.Motivo,
                movimiento.Observaciones,
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
