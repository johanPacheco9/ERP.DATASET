using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
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
        {
            throw new InvalidOperationException(errors);
        }

        //Obtener variante
        var variante = await _context.ProductoVariantes
            .AsNoTracking()
            .Where(v => v.Id == salida.ProductoVarianteId)
            .Select(v => new
            {
                v.Id,
                v.ProductoId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (variante == null)
        {
            throw new InvalidOperationException("La variante de producto no existe");
        }

        //Obtener stock actual
        var stock = await _context.StockBodegas
            .FirstOrDefaultAsync(
                s => s.BodegaId == salida.BodegaId &&
                     s.ProductoVarianteId == salida.ProductoVarianteId,
                cancellationToken);

        if (stock == null || stock.StockActual < salida.Cantidad)
        {
            throw new InvalidOperationException(
                $"Stock insuficiente. Disponible: {stock?.StockActual ?? 0}");
        }

        using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var movimiento = new Movimiento
            {
                BodegaId = salida.BodegaId,
                ProductoVarianteId = salida.ProductoVarianteId,
                ProductoId = variante.ProductoId,
                TipoMovimiento = TipoMovimiento.Salida,
                Cantidad = salida.Cantidad,
                CostoUnitario = 0m,
                Motivo = salida.Motivo,
                Observaciones = salida.Observaciones,
                ReferenciaTipo = salida.ReferenciaTipo,
                ReferenciaId = salida.ReferenciaId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "systemuser"
            };

            _context.Movimientos.Add(movimiento);

            stock.StockActual -= salida.Cantidad;
            stock.FechaActualizacion = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new MovimientoDetailDto
            (
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
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error registrando salida de inventario");
            throw;
        }
    }
}
