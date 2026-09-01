using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService
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

            foreach (var unitId in request.ProductIds)
            {
                var unidadReal = await context.UnidadesProductos
                    .Include(u => u.Bodega)
                    .Include(u => u.ProductoVariante)
                    .FirstOrDefaultAsync(u => u.Id == unitId, cancellationToken);

                if (unidadReal == null) continue;

                // Capturamos la bodega origen de la primera unidad procesada para el detalle
                if (bodegaOrigenId == null)
                {
                    bodegaOrigenId = (int?)unidadReal.BodegaId;
                    nombreBodegaOrigen = unidadReal.Bodega?.Name ?? unidadReal.Bodega?.Ubication;
                }

                // 1. Descontar stock de la bodega origen actual
                var stockOrigen = await context.WarehouseStock
                    .FirstOrDefaultAsync(s => s.WarehouseId == unidadReal.BodegaId && s.ProductoVarianteId == unidadReal.ProductoVarianteId, cancellationToken);

                if (stockOrigen != null && stockOrigen.CurrentStock > 0)
                {
                    stockOrigen.CurrentStock -= 1;
                }

                // 2. Actualizar ubicación y estado de la unidad física
                unidadReal.BodegaId = bodegaBajas.Id;
                unidadReal.Status = UnidadProductoStatus.Damaged; 

                // 3. Agregar al listado de items del movimiento
                itemsDto.Add(new MovimientoItemDto(
                    UnidadProductoId: (int)unidadReal.Id,
                    ProductoVarianteId: (int)unidadReal.ProductoVarianteId,
                    SerialNumber: unidadReal.SerialNumber,
                    Lote: unidadReal.Lote,
                    FechaVencimiento: unidadReal.FechaVencimiento ?? null
                ));
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Mapeo correcto utilizando el constructor posicional del record MovimientoDetailDto
            var resultadoDto = new MovimientoDetailDto(
                MovimientoId: 0, // Ajustar si registras una cabecera de movimiento en base de datos
                BodegaOrigenId: bodegaOrigenId,
                NombreBodegaOrigen: nombreBodegaOrigen,
                BodegaDestinoId: (int)bodegaBajas.Id,
                NombreBodegaDestino: bodegaBajas.Name ?? bodegaBajas.Ubication,
                TipoMovimiento: TipoMovimiento.Baja, // Asegúrate de tener este valor en tu enum
                Cantidad: itemsDto.Count,
                CostoUnitario: 0,
                CostoTotal: 0,
                ReferenciaId: null,
                ReferenciaTipo: null,
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