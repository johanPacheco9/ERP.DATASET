using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

internal static class StockHelper
{
    internal static async Task<int> DeductInventoryAsync(
        MainDataContext context,
        int warehouseId,
        int productoBaseId,
        int productoVarianteId,
        int quantity,
        string motivo,
        string createdBy,
        int? saleId,
        string? serialNumber,
        CancellationToken cancellationToken)
    {
        // 1. Obtener la variante y su producto base para resolver el costo (con fallback al padre)
        var variante = await context.ProductoVariantes
                           .AsNoTracking()
                           .Include(v => v.ProductoBase)
                           .FirstOrDefaultAsync(v => v.Id == productoVarianteId && v.ProductoBaseId == productoBaseId, cancellationToken)
                       ?? throw new InvalidOperationException($"La variante #{productoVarianteId} asociada al producto base #{productoBaseId} no existe."); 
        
        
        // Resolver costo aplicando fallback si el costo de la variante es 0 o nulo
        decimal costoAplicado = (variante.CostoUnitario.HasValue && variante.CostoUnitario.Value > 0)
            ? variante.CostoUnitario.Value
            : variante.ProductoBase.CostoUnitario;

        int? unidadProductoId = null;

        // 2. CASO 1: Venta por Serial / IMEI Único (UnidadProducto)
        if (!string.IsNullOrWhiteSpace(serialNumber))
        {
            if (quantity != 1)
                throw new InvalidOperationException("Si se vende una unidad física por serial único, la cantidad debe ser 1.");

            var unidad = await context.UnidadesProductos
                .FirstOrDefaultAsync(u => 
                    u.SerialNumber == serialNumber && 
                    u.ProductoVarianteId == productoVarianteId &&
                    u.BodegaId == warehouseId && 
                    u.Status == UnidadProductoStatus.Available, cancellationToken)
                ?? throw new InvalidOperationException($"El serial '{serialNumber}' no está disponible en la bodega especificada.");

            unidad.Status = UnidadProductoStatus.Sold;
            unidad.UpdatedAt = DateTime.UtcNow;
            unidad.UpdatedBy = createdBy;

            // Guardamos la referencia para el Kárdex
            unidadProductoId = unidad.Id;
        }

        // 3. CASO 2: Descontar Saldo Agregado de Inventario (WarehouseStock)
        var stock = await context.WarehouseStock
            .FirstOrDefaultAsync(s => 
                s.WarehouseId == warehouseId && 
                s.ProductoVarianteId == productoVarianteId, cancellationToken);

        if (stock == null || stock.CurrentStock < quantity)
        {
            throw new InvalidOperationException(
                $"Stock insuficiente en bodega. Disponible: {stock?.CurrentStock ?? 0}, solicitado: {quantity}.");
        }

        stock.CurrentStock -= quantity;
        stock.FechaActualizacion = DateTime.UtcNow;

        // 4. Registrar Movimiento en Kárdex
        var movimiento = new Movement
        {
            WarehouseId = warehouseId,
            ProductoVarianteId = productoVarianteId, // FK principal al SKU
            UnidadProductoId = unidadProductoId,      // FK opcional al Serial exacto
            Type = TipoMovimiento.Salida,
            Quantity = quantity,
            UnitCost = costoAplicado,
            ReferenceId = saleId,
            ReferenceType = "venta",
            Motive = motivo,
            Observations = saleId.HasValue ? $"Venta #{saleId}" : motivo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        context.Movements.Add(movimiento);

        // 5. Persistir cambios de forma atómica
        await context.SaveChangesAsync(cancellationToken);

        return movimiento.Id;
    }

    internal static async Task<int> GetAvailableCountAsync(
        MainDataContext context,
        int productoVarianteId,
        int warehouseId,
        CancellationToken cancellationToken)
    {
        return await context.WarehouseStock
            .Where(s => s.WarehouseId == warehouseId && s.ProductoVarianteId == productoVarianteId)
            .Select(s => s.CurrentStock)
            .FirstOrDefaultAsync(cancellationToken);
    }
}