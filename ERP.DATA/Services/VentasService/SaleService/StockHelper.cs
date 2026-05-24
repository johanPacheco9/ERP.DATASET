using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Services.VentasService.SaleService;

internal static class StockHelper
{
    internal static async Task<int> DeductInventoryAsync(
        MainDataContext context,
        int warehouseId,
        int lineaProductoId,
        int? productoId,
        int quantity,
        string motivo,
        string createdBy,
        int? saleId,
        CancellationToken cancellationToken)
    {
        List<Producto> unidades;

        if (productoId.HasValue)
        {
            var unit = await context.Productos
                           .FirstOrDefaultAsync(p =>
                                   p.Id == productoId.Value &&
                                   p.BodegaId == warehouseId &&
                                   p.LineaProductoId == lineaProductoId &&
                                   p.Status == ProductoStatus.Available,
                               cancellationToken)
                       ?? throw new InvalidOperationException($"La unidad {productoId} no está disponible en esta bodega.");

            if (quantity != 1)
                throw new InvalidOperationException("Si selecciona una unidad por serial, la cantidad debe ser 1.");

            unidades = [unit];
        }
        else
        {
            unidades = await context.Productos
                .Where(p =>
                    p.LineaProductoId == lineaProductoId &&
                    p.BodegaId == warehouseId &&
                    p.Status == ProductoStatus.Available)
                .OrderBy(p => p.CreatedAt)
                .ThenBy(p => p.Id)
                .Take(quantity)
                .ToListAsync(cancellationToken);

            if (unidades.Count < quantity)
                throw new InvalidOperationException(
                    $"Stock insuficiente. Disponible: {unidades.Count}, solicitado: {quantity}.");
        }

        var lineaCost = await context.LineaProductos
            .AsNoTracking()
            .Where(l => l.Id == lineaProductoId)
            .Select(l => l.CostoUnitario)
            .FirstAsync(cancellationToken);

        // 1. Prepara el Movement
        var movimiento = new Movement
        {
            WarehouseId = warehouseId,
            LineaProductoId = lineaProductoId,
            ProductId = unidades.First().Id,
            Type = TipoMovimiento.Salida,
            Quantity = unidades.Count,
            UnitCost = unidades.First().CostoUnitario ?? lineaCost,
            ReferenceId = saleId,
            ReferenceTye = "venta",
            Motive = motivo,
            Observations = saleId.HasValue ? $"Venta #{saleId}" : motivo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        context.Movements.Add(movimiento);

        foreach (var unidad in unidades)
        {
            unidad.Status = ProductoStatus.Sold;
            unidad.UpdatedAt = DateTime.UtcNow;
            unidad.UpdatedBy = createdBy;

            context.UnitProductMovements.Add(new UnitProductMovement
            {
                ProductoId = unidad.Id,
                Movimiento = movimiento,
                TipoMovimiento = TipoMovimiento.Salida,
                BodegaOrigenId = warehouseId,
                Motivo = motivo,
                Observaciones = $"Salida por venta — movimiento #{movimiento.Id}"
            });
        }

        // 3. Actualiza stock
        var stock = await context.WarehouseStock.FirstOrDefaultAsync(
            s => s.WarehouseId == warehouseId && s.LineaProductoId == lineaProductoId,
            cancellationToken);

        if (stock != null)
        {
            stock.CurrentStock = Math.Max(0, stock.CurrentStock - unidades.Count);
            stock.FechaActualizacion = DateTime.UtcNow;
        }

        // 4. Un solo SaveChanges guarda todo junto
        await context.SaveChangesAsync(cancellationToken);

        return movimiento.Id;
    }


    internal static async Task<int> GetAvailableCountAsync(
        MainDataContext context,
        int lineaProductoId,
        int warehouseId,
        CancellationToken cancellationToken) =>
        await context.Productos.CountAsync(p =>
                p.LineaProductoId == lineaProductoId &&
                p.BodegaId == warehouseId &&
                p.Status == ProductoStatus.Available,
            cancellationToken);
}