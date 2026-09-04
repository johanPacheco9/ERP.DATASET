using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.Movimientos;

public partial class MovimientosManager
{
    /// <summary>
    /// Registra un movimiento de inventario unificado (entrada, salida, baja,
    /// pérdida o transferencia entre bodegas) según el <see cref="TipoMovimiento"/> indicado.
    /// </summary>
    public async Task<Result> RegistrarMovimientoInventario(
        RegistrarMovimientoRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Validaciones generales
        if (request.ProductIds.Count == 0)
        {
            return Result.Failure(
                Error.Validation("Product.Empty", "Debe seleccionar al menos un producto.")
            );
        }

        if (request.TipoMovimiento is TipoMovimiento.SalidaTransferencia or TipoMovimiento.EntradaTransferencia)
        {
            return Result.Failure(
                Error.Validation(
                    "Movement.InvalidTipoMovimiento",
                    "Este tipo de movimiento es generado internamente y no puede solicitarse directamente."
                )
            );
        }

        var esTransferencia = request.TipoMovimiento == TipoMovimiento.Transferencia;
        var esPerdida = request.TipoMovimiento == TipoMovimiento.Perdida;

        if (esTransferencia || esPerdida)
        {
            if (!request.DestinationWarehouseId.HasValue)
            {
                return Result.Failure(
                    Error.Validation(
                        "Warehouse.DestinationRequired",
                        esTransferencia ? "La transferencia requiere una bodega destino." : "El registro de pérdida requiere especificar una bodega destino."
                    )
                );
            }

            if (request.OriginWarehouseId == request.DestinationWarehouseId.Value)
            {
                return Result.Failure(
                    Error.Validation(
                        "Warehouse.SameWarehouse",
                        "La bodega origen y destino no pueden ser la misma."
                    )
                );
            }
        }

        var productIds = request.ProductIds.Distinct().ToList();

        // 2. Bodega principal (origen del movimiento)
        var bodegaPrincipal = await context.Warehouse
            .FirstOrDefaultAsync(x => x.Id == request.OriginWarehouseId, cancellationToken);

        if (bodegaPrincipal is null)
        {
            return Result.Failure(Error.NotFound("Warehouse.NotFound", "No existe la bodega especificada."));
        }

        Warehouse? bodegaDestino = null;
        if (esTransferencia || esPerdida)
        {
            bodegaDestino = await context.Warehouse
                .FirstOrDefaultAsync(x => x.Id == request.DestinationWarehouseId!.Value, cancellationToken);

            if (bodegaDestino is null)
            {
                return Result.Failure(Error.NotFound("Warehouse.DestinationNotFound", "No existe la bodega destino especificada."));
            }
        }

        // 3. Validación de productos
        var productos = await context.UnidadesProductos
            .Where(x => productIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (productos.Count != productIds.Count)
        {
            return Result.Failure(Error.NotFound("Product.NotFound", "Uno o más productos no existen."));
        }

        if (productos.Any(x => x.BodegaId != request.OriginWarehouseId))
        {
            return Result.Failure(
                Error.Validation(
                    "Product.InvalidWarehouse",
                    "Uno o más productos no pertenecen a la bodega indicada."
                )
            );
        }

        if (request.TipoMovimiento != TipoMovimiento.Entrada &&
            productos.Any(x => x.Status != UnidadProductoStatus.Available))
        {
            return Result.Failure(
                Error.Validation(
                    "Product.InvalidStatus",
                    "Uno o más productos no están disponibles para este movimiento."
                )
            );
        }

        // 4. Stock físico de origen
        var variantIds = productos.Select(p => p.ProductoVarianteId).Distinct().ToList();

        var stockOrigenList = await context.WarehouseStock
            .Where(s => s.WarehouseId == request.OriginWarehouseId && variantIds.Contains(s.ProductoVarianteId))
            .ToListAsync(cancellationToken);

        if (request.TipoMovimiento != TipoMovimiento.Entrada)
        {
            foreach (var producto in productos)
            {
                var stockOrigen = stockOrigenList
                    .FirstOrDefault(s => s.ProductoVarianteId == producto.ProductoVarianteId);

                if (stockOrigen is null || stockOrigen.CurrentStock < 1)
                {
                    return Result.Failure(
                        Error.Validation(
                            "Warehouse.InsufficientStock",
                            $"No hay stock disponible para el producto {producto.Id} en la bodega origen."
                        )
                    );
                }
            }
        }

        List<WarehouseStock> stockDestinoList = [];
        if (esTransferencia || esPerdida)
        {
            stockDestinoList = await context.WarehouseStock
                .Where(s => s.WarehouseId == request.DestinationWarehouseId!.Value &&
                            variantIds.Contains(s.ProductoVarianteId))
                .ToListAsync(cancellationToken);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Creamos la cabecera global del movimiento que agrupará a todo el lote
            var movement = new Movement
            {
                OrigenWarehouseId = request.OriginWarehouseId,
                DestinationWarehouseId = request.DestinationWarehouseId,
                Type = request.TipoMovimiento,
                Quantity = productos.Count, // Cantidad total de ítems afectados en este lote
                UnitCost = 0,
                Observations = request.Observations,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            };

            context.Movements.Add(movement);
            await context.SaveChangesAsync(cancellationToken); // Genera el Id de la cabecera

            foreach (var producto in productos)
            {
                switch (request.TipoMovimiento)
                {
                    case TipoMovimiento.Transferencia:
                        RegistrarTransferenciaUnitaria(
                            request, producto, stockOrigenList, stockDestinoList, movement);
                        break;

                    case TipoMovimiento.Baja:
                        RegistrarSalidaDefinitivaUnitaria(
                            request, producto, stockOrigenList, UnidadProductoStatus.Damaged,
                            "Baja de inventario", movement);
                        break;

                    case TipoMovimiento.Perdida:
                        RegistrarPerdidaUnitaria(
                            request, producto, stockOrigenList, stockDestinoList, bodegaDestino!, movement);
                        break;

                    case TipoMovimiento.Entrada:
                        RegistrarEntradaUnitaria(request, producto, stockOrigenList, movement);
                        break;

                    case TipoMovimiento.Salida:
                        RegistrarSalidaGenericaUnitaria(request, producto, stockOrigenList, movement);
                        break;

                    default:
                        return Result.Failure(
                            Error.Validation(
                                "Movement.InvalidTipoMovimiento",
                                $"El tipo de movimiento '{request.TipoMovimiento}' no está soportado por este método."
                            )
                        );
                }
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure(
                Error.Failure(
                    "Inventory.MovementFailed",
                    "Ocurrió un error al registrar el movimiento de inventario."
                )
            );
        }
    }

    private string ObtenerReferenciaGeneral(TipoMovimiento tipo) => tipo switch
    {
        TipoMovimiento.Transferencia => "Transferencia entre bodegas",
        TipoMovimiento.Baja => "Baja de inventario",
        TipoMovimiento.Perdida => "Pérdida de inventario",
        TipoMovimiento.Entrada => "Entrada de inventario",
        TipoMovimiento.Salida => "Salida de inventario",
        _ => "Movimiento de inventario"
    };

    private void RegistrarTransferenciaUnitaria(
        RegistrarMovimientoRequest request,
        UnidadProducto producto,
        List<WarehouseStock> stockOrigenList,
        List<WarehouseStock> stockDestinoList,
        Movement movement)
    {
        var destinationWarehouseId = request.DestinationWarehouseId!.Value;

        context.UnitProductMovements.Add(new UnitProductMovement
        {
            UnidadProductoId = producto.Id,
            MovimientoId = movement.Id,
            TipoMovimiento = TipoMovimiento.Transferencia,
            BodegaOrigenId = request.OriginWarehouseId,
            BodegaDestinoId = destinationWarehouseId,
            Motivo = "Transferencia entre bodegas",
            Observaciones = request.Observations
        });

        var stockOrigen = stockOrigenList.First(s => s.ProductoVarianteId == producto.ProductoVarianteId);
        stockOrigen.CurrentStock -= 1;

        var stockDestino = stockDestinoList.FirstOrDefault(s => s.ProductoVarianteId == producto.ProductoVarianteId);
        if (stockDestino != null)
        {
            stockDestino.CurrentStock += 1;
        }
        else
        {
            stockDestino = new WarehouseStock
            {
                WarehouseId = destinationWarehouseId,
                ProductoVarianteId = producto.ProductoVarianteId,
                CurrentStock = 1
            };
            context.WarehouseStock.Add(stockDestino);
            stockDestinoList.Add(stockDestino);
        }

        producto.BodegaId = destinationWarehouseId;
    }

    private void RegistrarSalidaDefinitivaUnitaria(
        RegistrarMovimientoRequest request,
        UnidadProducto producto,
        List<WarehouseStock> stockOrigenList,
        UnidadProductoStatus estadoFinal,
        string motivo,
        Movement movement)
    {
        context.UnitProductMovements.Add(new UnitProductMovement
        {
            UnidadProductoId = producto.Id,
            MovimientoId = movement.Id,
            TipoMovimiento = request.TipoMovimiento,
            BodegaOrigenId = request.OriginWarehouseId,
            BodegaDestinoId = null,
            Motivo = motivo,
            Observaciones = request.Observations
        });

        var stockOrigen = stockOrigenList.First(s => s.ProductoVarianteId == producto.ProductoVarianteId);
        stockOrigen.CurrentStock -= 1;

        producto.Status = estadoFinal;
    }

    private void RegistrarEntradaUnitaria(
        RegistrarMovimientoRequest request,
        UnidadProducto producto,
        List<WarehouseStock> stockOrigenList,
        Movement movement)
    {
        context.UnitProductMovements.Add(new UnitProductMovement
        {
            UnidadProductoId = producto.Id,
            MovimientoId = movement.Id,
            TipoMovimiento = TipoMovimiento.Entrada,
            BodegaOrigenId = request.OriginWarehouseId,
            BodegaDestinoId = null,
            Motivo = "Entrada de inventario",
            Observaciones = request.Observations
        });

        var stockOrigen = stockOrigenList.FirstOrDefault(s => s.ProductoVarianteId == producto.ProductoVarianteId);
        if (stockOrigen != null)
        {
            stockOrigen.CurrentStock += 1;
        }
        else
        {
            stockOrigen = new WarehouseStock
            {
                WarehouseId = request.OriginWarehouseId,
                ProductoVarianteId = producto.ProductoVarianteId,
                CurrentStock = 1
            };
            context.WarehouseStock.Add(stockOrigen);
            stockOrigenList.Add(stockOrigen);
        }

        producto.Status = UnidadProductoStatus.Available;
    }

    private void RegistrarSalidaGenericaUnitaria(
        RegistrarMovimientoRequest request,
        UnidadProducto producto,
        List<WarehouseStock> stockOrigenList,
        Movement movement)
    {
        context.UnitProductMovements.Add(new UnitProductMovement
        {
            UnidadProductoId = producto.Id,
            MovimientoId = movement.Id,
            TipoMovimiento = TipoMovimiento.Salida,
            BodegaOrigenId = request.OriginWarehouseId,
            BodegaDestinoId = null,
            Motivo = "Salida de inventario",
            Observaciones = request.Observations
        });

        var stockOrigen = stockOrigenList.First(s => s.ProductoVarianteId == producto.ProductoVarianteId);
        stockOrigen.CurrentStock -= 1;
    }
    
    private void RegistrarPerdidaUnitaria(
        RegistrarMovimientoRequest request,
        UnidadProducto producto,
        List<WarehouseStock> stockOrigenList,
        List<WarehouseStock> stockDestinoList,
        Warehouse bodegaPerdidas,
        Movement movement)
    {
        var destinationWarehouseId = bodegaPerdidas.Id;

        context.UnitProductMovements.Add(new UnitProductMovement
        {
            UnidadProductoId = producto.Id,
            MovimientoId = movement.Id,
            TipoMovimiento = TipoMovimiento.Perdida,
            BodegaOrigenId = request.OriginWarehouseId,
            BodegaDestinoId = destinationWarehouseId,
            Motivo = "Pérdida de inventario",
            Observaciones = request.Observations
        });

        var stockOrigen = stockOrigenList.First(s => s.ProductoVarianteId == producto.ProductoVarianteId);
        stockOrigen.CurrentStock -= 1;

        var stockDestino = stockDestinoList.FirstOrDefault(s => s.ProductoVarianteId == producto.ProductoVarianteId);
        if (stockDestino != null)
        {
            stockDestino.CurrentStock += 1;
        }
        else
        {
            stockDestino = new WarehouseStock
            {
                WarehouseId = destinationWarehouseId,
                ProductoVarianteId = producto.ProductoVarianteId,
                CurrentStock = 1
            };
            context.WarehouseStock.Add(stockDestino);
            stockDestinoList.Add(stockDestino);
        }

        producto.BodegaId = destinationWarehouseId;
        producto.Status = UnidadProductoStatus.Lost;
    }
}