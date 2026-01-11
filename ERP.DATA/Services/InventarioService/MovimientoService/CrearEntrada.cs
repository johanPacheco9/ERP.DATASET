using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

/// <summary>
/// Registra movimiento de ingreso de productos a una bodega
/// </summary>
public partial class MovimientoService
{
    public async Task<int> RegistrarEntradaAsync(
        CreateEntryMovementRequest createEntry,
        CancellationToken cancellationToken)
    {
        if (createEntry.Cantidad <= 0)
        {
            _logger.LogWarning("Cantidad inválida: {Cantidad}", createEntry.Cantidad);
            return -2;
        }

        var variante = await _context.ProductoVariantes
            .AsNoTracking()
            .Where(v => v.Id == createEntry.ProductoVarianteId)
            .Select(v => new
            {
                v.Id,
                v.Costo_Unitario,
                v.Lote,
                v.Fecha_Vencimiento,
                v.ProductoId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (variante == null)
        {
            _logger.LogWarning(
                "ProductoVariante {VarianteId} no existe.",
                createEntry.ProductoVarianteId);
            return -1;
        }

        using var tx = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var movimiento = new Movimiento
            {
                BodegaId = createEntry.BodegaId,
                ProductoVarianteId = variante.Id,
                ProductoId = variante.ProductoId,
                TipoMovimiento = TipoMovimiento.Entrada,
                Cantidad = createEntry.Cantidad,
                CostoUnitario = variante.Costo_Unitario ?? 0m,
                CreatedAt = DateTime.UtcNow,
                Motivo = createEntry.Motivo,
                Lote = variante.Lote,
                FechaVencimiento = variante.Fecha_Vencimiento,
                CreatedBy = "systemuser"
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync(cancellationToken);

            // 2. Actualizar stock
            var stock = await _context.StockBodegas
                .FirstOrDefaultAsync(
                    s => s.BodegaId == createEntry.BodegaId &&
                         s.ProductoVarianteId == variante.Id,
                    cancellationToken);

            if (stock == null)
            {
                stock = new StockBodega
                {
                    BodegaId = createEntry.BodegaId,
                    ProductoVarianteId = variante.Id,
                    ProductoId = variante.ProductoId,
                    StockActual = createEntry.Cantidad,
                    FechaActualizacion = DateTime.UtcNow,
                    CreatedBy = "SystemUser",
                    CreatedAt = DateTime.UtcNow
                };
                _context.StockBodegas.Add(stock);
            }
            else
            {
                stock.StockActual += createEntry.Cantidad;
                stock.FechaActualizacion = DateTime.UtcNow;
            }

            // 3. Crear las unidades con sus movimientos
            var unidades = new List<UnitProduct>();
            var unitProductMovements = new List<UnitProductMovement>();

            for (int i = 0; i < createEntry.Cantidad; i++)
            {
                var unitProduct = new UnitProduct
                {
                    ProductoId = variante.ProductoId,
                    ProductoVarianteId = variante.Id,
                    BodegaId = createEntry.BodegaId,
                    UnitProductStatus = UnitProductStatus.Available,
                    Serial = Guid.NewGuid().ToString("N").ToUpper(),
                    FechaIngreso = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "systemuser"    
                };

                unidades.Add(unitProduct);
            }

            _context.UnitProduct.AddRange(unidades);
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Crear registros en UnitProductMovement
            foreach (var unidad in unidades)
            {
                var unitMovement = new UnitProductMovement
                {
                    ProductoUnidadId = unidad.Id,
                    TipoMovimiento = TipoMovimiento.Entrada,
                    BodegaDestinoId = createEntry.BodegaId,
                    BodegaOrigenId = 8,
                    Motivo = createEntry.Motivo,
                    Observaciones = $"Entrada por movimiento #{movimiento.Id}"
                };
                unitProductMovements.Add(unitMovement);
            }
            _context.UnitProductMovements.AddRange(unitProductMovements);
            await _context.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);

            return movimiento.Id;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Error registrando entrada de inventario");
            throw;
        }
    }
}