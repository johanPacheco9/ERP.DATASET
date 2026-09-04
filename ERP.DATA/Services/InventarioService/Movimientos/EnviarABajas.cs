using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.Movimientos;

public partial class MovimientosManager
{
    public async Task<Result<MovimientoDetailDto>> EnviarABaja(RegistrarMovimientoRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StoreId <= 0 || !request.ProductIds.Any())
        {
            return Result<MovimientoDetailDto>.Failure(
                Error.Validation("Product.Empty", "El ID de la tienda y las unidades a dar de baja son obligatorios.")
            );
        }

        var bodegaBajas = await context.Warehouse
            .FirstOrDefaultAsync(s => s.StoreId == request.StoreId && s.Type == WarehouseType.LossWarehouse, cancellationToken);

        if (bodegaBajas == null)
        {
            return Result<MovimientoDetailDto>.Failure(
                Error.NotFound("Warehouse.NotFound", "No se encontró una bodega de bajas configurada para esta tienda.")
            );
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var itemsDto = new List<MovimientoItemDto>();
            int? bodegaOrigenId = null;
            string? nombreBodegaOrigen = null;
            decimal costoTotal = 0;

            // 1. Crear cabecera Movement primero
            var movimiento = new ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory.Movement
            {
                DestinationWarehouseId = bodegaBajas.Id,
                Type = TipoMovimiento.Baja,
                Quantity = request.ProductIds.Count,
                UnitCost = 0,
                Motive = "Baja de inventario",
                Observations = request.Observations ?? "Baja generada por daño o inconsistencia",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1
            };
            
            context.Movements.Add(movimiento);
            await context.SaveChangesAsync(cancellationToken); // Para obtener el Id

            foreach (var unitId in request.ProductIds)
            {
                var unidadReal = await context.UnidadesProductos
                    .Include(u => u.Bodega)
                    .Include(u => u.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
                    .FirstOrDefaultAsync(u => u.Id == unitId, cancellationToken);

                if (unidadReal == null) continue;

                if (bodegaOrigenId == null)
                {
                    bodegaOrigenId = (int?)unidadReal.BodegaId;
                    nombreBodegaOrigen = unidadReal.Bodega?.Name ?? unidadReal.Bodega?.Ubication;
                    movimiento.OrigenWarehouseId = bodegaOrigenId ?? 0;
                }

                // 2. Descontar stock
                var stockOrigen = await context.WarehouseStock
                    .FirstOrDefaultAsync(s => s.WarehouseId == unidadReal.BodegaId && s.ProductoVarianteId == unidadReal.ProductoVarianteId, cancellationToken);

                if (stockOrigen != null && stockOrigen.CurrentStock > 0)
                {
                    stockOrigen.CurrentStock -= 1;
                }
                
                decimal unitCost = unidadReal.ProductoVariante?.CostoUnitario > 0 
                    ? unidadReal.ProductoVariante.CostoUnitario.Value 
                    : (unidadReal.ProductoVariante?.ProductoBase?.CostoUnitario ?? 0m);
                costoTotal += unitCost;

                // 3. Registrar en UnitProductMovements
                context.UnitProductMovements.Add(new ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts.UnitProductMovement
                {
                    UnidadProductoId = unidadReal.Id,
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = TipoMovimiento.Baja,
                    BodegaOrigenId = unidadReal.BodegaId,
                    BodegaDestinoId = bodegaBajas.Id,
                    Motivo = request.Observations ?? "Baja de unidad",
                    Observaciones = ""
                });

                // 4. Actualizar unidad
                unidadReal.BodegaId = bodegaBajas.Id;
                unidadReal.Status = UnidadProductoStatus.Damaged; 

                itemsDto.Add(new MovimientoItemDto(
                    UnidadProductoId: (int)unidadReal.Id,
                    ProductoVarianteId: unidadReal.ProductoVarianteId,
                    SerialNumber: unidadReal.SerialNumber,
                    Lote: unidadReal.Lote,
                    FechaVencimiento: unidadReal.FechaVencimiento ?? null
                ));
            }

            movimiento.UnitCost = itemsDto.Count > 0 ? costoTotal / itemsDto.Count : 0;
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var resultadoDto = new MovimientoDetailDto(
                MovimientoId: movimiento.Id,
                BodegaOrigenId: bodegaOrigenId,
                NombreBodegaOrigen: nombreBodegaOrigen,
                BodegaDestinoId: (int)bodegaBajas.Id,
                NombreBodegaDestino: bodegaBajas.Name ?? bodegaBajas.Ubication,
                TipoMovimiento: TipoMovimiento.Baja, // Asegúrate de tener este valor en tu enum
                Cantidad: itemsDto.Count,
                CostoUnitario: 0,
                CostoTotal: 0,
                Lote: null,
                FechaVencimiento: null,
                Motivo: request.Observations ?? "Baja de inventario por auditoría",
                Observaciones: request.Observations ?? "",
                Items: itemsDto,
                CreatedAt: DateTime.UtcNow,
                CreatedBy: "Sistema"
            );

            return Result<MovimientoDetailDto>.Success(resultadoDto);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<MovimientoDetailDto>.Failure(
                Error.Failure("Movimiento.Error", $"Error al procesar las bajas: {ex.Message}")
            );
        }
    }
}