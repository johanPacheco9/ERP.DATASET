using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

internal static class StockHelper
{
    // Número máximo de reintentos ante un conflicto de concurrencia.
    private const int MaxConcurrencyRetries = 3;

 internal static async Task<(int MovementId, int? UnidadProductoId, decimal CostoVentaTotal)> DeductInventoryAsync(
        MainDataContext context,
        int warehouseId,
        int productoBaseId,
        int productoVarianteId,
        int quantity,
        string motivo,
        int createdBy,
        int? saleId,
        string? serialNumber,
        CancellationToken cancellationToken)
    {
        // 1. Obtener la variante y su producto base para resolver el costo
        var variante = await context.ProductoVariantes
                     .AsNoTracking()
                     .Include(v => v.ProductoBase)
                     .FirstOrDefaultAsync(
                          v => v.Id == productoVarianteId &&
                               v.ProductoBaseId == productoBaseId,
                          cancellationToken)
                   ?? throw new InvalidOperationException(
                       $"La variante #{productoVarianteId} asociada al producto base " +
                       $"#{productoBaseId} no existe.");

        decimal costoAplicado =
            (variante.CostoUnitario.HasValue && variante.CostoUnitario.Value > 0)
                ? variante.CostoUnitario.Value
                : variante.ProductoBase.CostoUnitario;

        // 2. CASO 1: Venta por Serial / IMEI único (No aplica FIFO por lotes).
        if (!string.IsNullOrWhiteSpace(serialNumber))
        {
            if (quantity != 1)
            {
                throw new InvalidOperationException(
                    "Si se vende una unidad física por serial único, " +
                    "la cantidad debe ser 1.");
            }

            var unidad = await context.UnidadesProductos
                           .FirstOrDefaultAsync(
                                u => u.SerialNumber == serialNumber &&
                                     u.ProductoVarianteId == productoVarianteId &&
                                     u.BodegaId == warehouseId &&
                                     u.Status == UnidadProductoStatus.Available,
                                cancellationToken)
                       ?? throw new InvalidOperationException(
                           $"El serial '{serialNumber}' no está disponible " +
                           $"en la bodega especificada.");

            var unidadMasAntigua = await context.UnidadesProductos
                .Where(u => u.ProductoVarianteId == productoVarianteId &&
                            u.BodegaId == warehouseId &&
                            u.Status == UnidadProductoStatus.Available)
                .OrderBy(u => u.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            bool ventaFueraDeOrdenFifo = unidadMasAntigua != null && unidadMasAntigua.Id != unidad.Id;

            // ==========================================
            // VALIDACIÓN DE PARÁMETRO: FIFO Estricto
            // ==========================================
            if (ventaFueraDeOrdenFifo)
            {
                // Consultamos el parámetro global de la empresa/sistema
                var parametros = await context.Parametros.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
                bool forzarFifoEstricto = parametros?.ForzarFifoEstricto ?? false;

                if (forzarFifoEstricto)
                {
                    throw new InvalidOperationException(
                        $"No se puede despachar el serial '{serialNumber}'. " +
                        $"Por políticas de FIFO estricto de la empresa, debe despachar primero " +
                        $"el serial más antiguo disponible ('{unidadMasAntigua!.SerialNumber}', ingresado el {unidadMasAntigua.CreatedAt:yyyy-MM-dd}).");
                }
            }

            unidad.Status = UnidadProductoStatus.Sold;
            unidad.UpdatedAt = DateTime.UtcNow;
            unidad.UpdatedBy = createdBy;

            var unidadProductoId = unidad.Id;

            // Descontar stock agregado con control de concurrencia.
            await DeductWarehouseStockWithRetryAsync(
                context,
                warehouseId,
                productoVarianteId,
                quantity,
                cancellationToken);

            // Crear cabecera del movimiento de Kardex.
            var observacionesSerial = saleId.HasValue
                ? $"Venta #{saleId}"
                : motivo;

            if (ventaFueraDeOrdenFifo)
            {
                observacionesSerial += $" | Advertencia FIFO: se despachó el serial '{serialNumber}' " +
                                       $"en vez del más antiguo disponible (serial '{unidadMasAntigua!.SerialNumber}', " +
                                       $"ingresado el {unidadMasAntigua.CreatedAt:yyyy-MM-dd}).";
            }

            var movimientoSerial = new Movement
            {
                OrigenWarehouseId = warehouseId,
                ProductoVarianteId = productoVarianteId,
                Type = TipoMovimiento.Salida,
                Quantity = quantity,
                UnitCost = costoAplicado,
                SaleId = saleId,
                Motive = motivo,
                Observations = observacionesSerial,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            context.Movements.Add(movimientoSerial);
            await context.SaveChangesAsync(cancellationToken);

            context.UnitProductMovements.Add(new UnitProductMovement
            {
                UnidadProductoId = unidadProductoId,
                MovimientoId = movimientoSerial.Id,
                TipoMovimiento = TipoMovimiento.Salida,
                BodegaOrigenId = warehouseId,
                BodegaDestinoId = null,
                Motivo = motivo,
                Observaciones = observacionesSerial
            });

            await context.SaveChangesAsync(cancellationToken);

            return (movimientoSerial.Id, unidadProductoId, costoAplicado * quantity);
        }

        // 3. CASO 2: Venta de productos NO serializados (Consumo FIFO real)...
        // (El resto del código continúa exactamente igual)
        
        await DeductWarehouseStockWithRetryAsync(
            context,
            warehouseId,
            productoVarianteId,
            quantity,
            cancellationToken);

        var movimientoSalida = new Movement
        {
            OrigenWarehouseId = warehouseId,
            ProductoVarianteId = productoVarianteId,
            Type = TipoMovimiento.Salida,
            Quantity = quantity,
            UnitCost = 0m,
            SaleId = saleId,
            Motive = motivo,
            Observations = saleId.HasValue ? $"Venta #{saleId}" : motivo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        context.Movements.Add(movimientoSalida);
        await context.SaveChangesAsync(cancellationToken);

        var entradasDisponibles = await context.Movements
            .Where(m => m.OrigenWarehouseId == warehouseId &&
                        m.ProductoVarianteId == productoVarianteId &&
                        m.Type == TipoMovimiento.Entrada &&
                        m.RemainingQuantity.HasValue &&
                        m.RemainingQuantity.Value > 0)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        int cantidadPendiente = quantity;
        decimal costoTotalConsumido = 0m;

        foreach (var entrada in entradasDisponibles)
        {
            if (cantidadPendiente <= 0) break;

            int disponible = entrada.RemainingQuantity!.Value;
            int aConsumir = Math.Min(disponible, cantidadPendiente);

            entrada.RemainingQuantity = disponible - aConsumir;
            entrada.UpdatedAt = DateTime.UtcNow;
            entrada.UpdatedBy = createdBy;

            var consumo = new MovementConsumption
            {
                ExitMovementId = movimientoSalida.Id,
                EntryMovementId = entrada.Id,
                QuantityConsumed = aConsumir,
                UnitCost = entrada.UnitCost,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            context.MovementConsumptions.Add(consumo);

            costoTotalConsumido += aConsumir * entrada.UnitCost;
            cantidadPendiente -= aConsumir;
        }

        if (cantidadPendiente > 0)
        {
            costoTotalConsumido += cantidadPendiente * costoAplicado;
            string notaEstimada = $"Costo parcial estimado: {cantidadPendiente} unidades sin trazabilidad FIFO";
            movimientoSalida.Observations = string.IsNullOrWhiteSpace(movimientoSalida.Observations)
                ? notaEstimada
                : $"{movimientoSalida.Observations} | {notaEstimada}";
        }

        movimientoSalida.UnitCost = quantity > 0 ? Math.Round(costoTotalConsumido / quantity, 4) : 0m;
        movimientoSalida.UpdatedAt = DateTime.UtcNow;
        movimientoSalida.UpdatedBy = createdBy;

        await context.SaveChangesAsync(cancellationToken);

        return (movimientoSalida.Id, null, costoTotalConsumido);
    }

    // Descuenta stock con reintentos ante conflictos de concurrencia.
    private static async Task DeductWarehouseStockWithRetryAsync(
        MainDataContext context,
        int warehouseId,
        int productoVarianteId,
        int quantity,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1;
             attempt <= MaxConcurrencyRetries;
             attempt++)
        {
            var stock = await context.WarehouseStock
                .FirstOrDefaultAsync(
                    s => s.WarehouseId == warehouseId &&
                         s.ProductoVarianteId == productoVarianteId,
                    cancellationToken);

            if (stock == null || stock.CurrentStock < quantity)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente en bodega. " +
                    $"Disponible: {stock?.CurrentStock ?? 0}, " +
                    $"solicitado: {quantity}.");
            }

            stock.CurrentStock -= quantity;
            stock.FechaActualizacion = DateTime.UtcNow;

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return;
            }
            catch (DbUpdateConcurrencyException)
                when (attempt < MaxConcurrencyRetries)
            {
                // La fila cambió antes de guardar.
                // Desconectamos la entidad y volvemos a leer el stock actualizado.
                context.Entry(stock).State = EntityState.Detached;
            }
        }

        throw new InvalidOperationException(
            $"No se pudo actualizar el stock de la variante " +
            $"#{productoVarianteId} en la bodega #{warehouseId} " +
            $"tras {MaxConcurrencyRetries} intentos por alta concurrencia. " +
            $"Intente la venta nuevamente.");
    }

    internal static async Task<int> GetAvailableCountAsync(
        MainDataContext context,
        int productoVarianteId,
        int warehouseId,
        CancellationToken cancellationToken)
    {
        return await context.WarehouseStock
            .Where(s =>
                s.WarehouseId == warehouseId &&
                s.ProductoVarianteId == productoVarianteId)
            .Select(s => s.CurrentStock)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Contraparte de DeductInventoryAsync.
    // Se utiliza para anulaciones y devoluciones.
    internal static async Task ReturnInventoryAsync(
        MainDataContext context,
        int warehouseId,
        int productoVarianteId,
        int? unidadProductoId,
        int quantity,
        string motivo,
        int createdBy,
        int? saleId,
        CancellationToken cancellationToken)
    {
        // 1. Obtener la variante y su producto base para resolver el costo.
        var variante = await context.ProductoVariantes
                           .AsNoTracking()
                           .Include(v => v.ProductoBase)
                           .FirstOrDefaultAsync(
                               v => v.Id == productoVarianteId,
                               cancellationToken)
                       ?? throw new InvalidOperationException(
                           $"La variante #{productoVarianteId} no existe.");

        decimal costoAplicado =
            (variante.CostoUnitario.HasValue && variante.CostoUnitario.Value > 0)
                ? variante.CostoUnitario.Value
                : variante.ProductoBase.CostoUnitario;

        // 2. Si corresponde a una unidad serializada,
        //    devolverla al estado disponible.
        if (unidadProductoId.HasValue)
        {
            var unidad = await context.UnidadesProductos
                .FirstOrDefaultAsync(
                    u => u.Id == unidadProductoId.Value,
                    cancellationToken);

            if (unidad != null)
            {
                unidad.Status = UnidadProductoStatus.Available;
                unidad.UpdatedAt = DateTime.UtcNow;
                unidad.UpdatedBy = createdBy;
            }
        }

        // 3. Devolver stock agregado con control de concurrencia.
        await ReturnWarehouseStockWithRetryAsync(
            context,
            warehouseId,
            productoVarianteId,
            quantity,
            cancellationToken);

        // 4. Registrar movimiento de Entrada en el Kardex.
        var movimiento = new Movement
        {
            OrigenWarehouseId = warehouseId,
            ProductoVarianteId = productoVarianteId,
            Type = TipoMovimiento.Entrada,
            Quantity = quantity,
            RemainingQuantity = quantity,
            UnitCost = costoAplicado,
            SaleId = saleId,
            Motive = motivo,
            Observations = saleId.HasValue
                ? $"Anulación venta #{saleId}"
                : motivo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        context.Movements.Add(movimiento);

        await context.SaveChangesAsync(cancellationToken);

        // 5. Registrar detalle si la unidad era serializada.
        if (unidadProductoId.HasValue)
        {
            context.UnitProductMovements.Add(new UnitProductMovement
            {
                UnidadProductoId = unidadProductoId.Value,
                MovimientoId = movimiento.Id,
                TipoMovimiento = TipoMovimiento.Entrada,
                BodegaOrigenId = warehouseId,
                BodegaDestinoId = warehouseId,
                Motivo = motivo,
                Observaciones = saleId.HasValue
                    ? $"Anulación venta #{saleId}"
                    : motivo
            });

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    // Devuelve stock con reintentos ante conflictos de concurrencia.
    private static async Task ReturnWarehouseStockWithRetryAsync(
        MainDataContext context,
        int warehouseId,
        int productoVarianteId,
        int quantity,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1;
             attempt <= MaxConcurrencyRetries;
             attempt++)
        {
            var stock = await context.WarehouseStock
                            .FirstOrDefaultAsync(
                                s => s.WarehouseId == warehouseId &&
                                     s.ProductoVarianteId == productoVarianteId,
                                cancellationToken)
                        ?? throw new InvalidOperationException(
                            $"No existe registro de stock para la variante " +
                            $"#{productoVarianteId} en la bodega #{warehouseId}.");

            stock.CurrentStock += quantity;
            stock.FechaActualizacion = DateTime.UtcNow;

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return;
            }
            catch (DbUpdateConcurrencyException)
                when (attempt < MaxConcurrencyRetries)
            {
                context.Entry(stock).State = EntityState.Detached;
            }
        }

        throw new InvalidOperationException(
            $"No se pudo devolver el stock de la variante " +
            $"#{productoVarianteId} en la bodega #{warehouseId} " +
            $"tras {MaxConcurrencyRetries} intentos por alta concurrencia. " +
            $"Intente la anulación nuevamente.");
    }
}