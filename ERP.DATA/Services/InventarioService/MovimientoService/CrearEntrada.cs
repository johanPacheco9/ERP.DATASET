using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService
{
    public async Task<int> RegistrarEntradaAsync(
        RegistrarMovimientoEntradaRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(request.Cantidad));

        // 1. Validar la variante y obtener su costo base
        var variante = await context.ProductoVariantes
            .Include(v => v.ProductoBase)
            .FirstOrDefaultAsync(v => v.Id == request.ProductoVarianteId, cancellationToken)
            ?? throw new KeyNotFoundException($"La Variante #{request.ProductoVarianteId} no existe.");

        decimal costoAplicado = (variante.CostoUnitario > 0 ? variante.CostoUnitario : variante.ProductoBase.CostoUnitario) ?? 0m;

        using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 2. Crear la Cabecera del Movimiento (Kárdex global)
            var movimiento = new Movement
            {
                OrigenWarehouseId = request.BodegaId, 
                Type = TipoMovimiento.Entrada,
                Quantity = request.Cantidad,
                UnitCost = costoAplicado,
                Lote = request.Lote,
                FechaVencimiento = request.FechaVencimiento,
                Motive = request.Motivo,
                Observations = request.Motivo,
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow
            };

            context.Movements.Add(movimiento);
            await context.SaveChangesAsync(cancellationToken); // Guardar para generar el Id de la cabecera

            // 3. Incrementar o crear el saldo general en WarehouseStock
            var stock = await context.WarehouseStock
                .FirstOrDefaultAsync(s => 
                    s.WarehouseId == request.BodegaId && 
                    s.ProductoVarianteId == variante.Id, cancellationToken);

            if (stock == null)
            {
                stock = new WarehouseStock
                {
                    WarehouseId = request.BodegaId,
                    ProductoVarianteId = variante.Id,
                    CurrentStock = request.Cantidad,
                    FechaActualizacion = DateTime.UtcNow,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                };
                context.WarehouseStock.Add(stock);
            }
            else
            {
                stock.CurrentStock += request.Cantidad;
                stock.FechaActualizacion = DateTime.UtcNow;
            }

            // 4. Manejo de unidades físicas rastreables si aplica
            if (variante.ProductoBase.RequiereSerial || (request.Seriales != null && request.Seriales.Any()))
            {
                var serialesList = request.Seriales ?? [];

                var unidades = Enumerable.Range(0, request.Cantidad).Select(i => new UnidadProducto
                {
                    ProductoVarianteId = variante.Id,
                    BodegaId = request.BodegaId,
                    SerialNumber = i < serialesList.Count ? serialesList[i] : Guid.NewGuid().ToString("N").ToUpper(),
                    Lote = request.Lote,
                    FechaVencimiento = request.FechaVencimiento,
                    Status = UnidadProductoStatus.Available,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                context.UnidadesProductos.AddRange(unidades);
                await context.SaveChangesAsync(cancellationToken); // Guardar para obtener los IDs de las unidades físicas

                // Registrar cada unidad creada en la tabla intermedia de detalles (UnitProductMovement)
                foreach (var unidad in unidades)
                {
                    context.UnitProductMovements.Add(new UnitProductMovement
                    {
                        UnidadProductoId = unidad.Id,
                        MovimientoId = movimiento.Id,
                        TipoMovimiento = TipoMovimiento.Entrada,
                        BodegaOrigenId = request.BodegaId,
                        BodegaDestinoId = null,
                        Motivo = request.Motivo ?? "Entrada de inventario",
                        Observaciones = request.Motivo
                    });
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return movimiento.Id;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Error registrando entrada para Variante {Id}", request.ProductoVarianteId);
            throw;
        }
    }
}