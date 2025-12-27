using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

/// <summary>
/// Registra movimiento entrada de productos a una bodega
/// </summary>
public partial class MovimientoService
{
    public async Task<bool> RegistrarEntradaAsync(Movimiento entrada, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el producto exista
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == entrada.ProductoId, cancellationToken);

            if (producto == null)
            {
                _logger.LogWarning($"Producto {entrada.ProductoId} no existe.");
                return false;
            }

            // 2. Crear el movimiento
            var movimiento = new Movimiento
            {
                BodegaId = entrada.BodegaId,
                ProductoId = entrada.ProductoId,
                TipoMovimiento = TipoMovimiento.Entrada,
                Cantidad = entrada.Cantidad,
                CostoUnitario = producto.Costo_Unitario,
                CreatedAt = DateTime.UtcNow,
                Motivo = entrada.Motivo,
                Lote = entrada.Lote,
                FechaVencimiento = entrada.FechaVencimiento
            };

            _context.Movimientos.Add(movimiento);

            // 3. Actualizar StockBodega
            var stock = await _context.StockBodegas
                .FirstOrDefaultAsync(s =>
                    s.BodegaId == entrada.BodegaId &&
                    s.ProductoId == entrada.ProductoId,
                    cancellationToken
                );

            if (stock == null)
            {
                stock = new StockBodega
                {
                    BodegaId = entrada.BodegaId,
                    ProductoId = entrada.ProductoId,
                    StockActual = entrada.Cantidad,
                    FechaActualizacion = DateTime.UtcNow
                };
                _context.StockBodegas.Add(stock);
            }
            else
            {
                stock.StockActual += entrada.Cantidad;
                stock.FechaActualizacion = DateTime.UtcNow;
            }
            
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando movimiento de entrada");
            return false;
        }
    }
}

