using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

/// <summary>Registra movimiento de ingreso de productos a una bodega.</summary>
public partial class MovimientoService
{
    public async Task<int> RegistrarEntradaAsync(
        RegistrarMovimientoEntradaRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(request.Cantidad));

        var lineaProducto = await context.LineaProductos
                                .AsNoTracking()
                                .Where(v => v.Id == request.LineaProductoId)
                                .Select(v => new { v.Id, v.CostoUnitario })
                                .FirstOrDefaultAsync(cancellationToken)
                            ?? throw new KeyNotFoundException($"LineaProducto {request.LineaProductoId} no existe.");

        using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Crear movimiento cabecera
            var movimiento = new Movement
            {
                WarehouseId = request.BodegaId,
                LineaProductoId = lineaProducto.Id,
                Type = TipoMovimiento.Entrada,
                Quantity = request.Cantidad,
                UnitCost = lineaProducto.CostoUnitario,
                Motive = request.Motivo,
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow
            };

            context.Movements.Add(movimiento);
            await context.SaveChangesAsync(cancellationToken); // necesario para obtener movimiento.Id

            // 2. Crear productos (unidades físicas)
            var productos = Enumerable.Range(0, request.Cantidad)
                .Select(_ => new Producto
                {
                    LineaProductoId = lineaProducto.Id,
                    BodegaId = request.BodegaId,
                    Lote = request.Lote,
                    FechaVencimiento = request.FechaVencimiento,
                    Serial = request.RequiereSerial == true
                        ? Guid.NewGuid().ToString("N").ToUpper()
                        : null,
                    Status = ProductoStatus.Available,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            context.Productos.AddRange(productos);
            await context.SaveChangesAsync(cancellationToken); // necesario para obtener productos.Id

            // 3. Vincular cada unidad al movimiento
            context.UnitProductMovements.AddRange(productos.Select(p => new UnitProductMovement
            {
                ProductoId = p.Id,
                MovimientoId = movimiento.Id
            }));

            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return movimiento.Id;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Error registrando entrada para LineaProducto {Id}", request.LineaProductoId);

            throw;
        }
    }
}