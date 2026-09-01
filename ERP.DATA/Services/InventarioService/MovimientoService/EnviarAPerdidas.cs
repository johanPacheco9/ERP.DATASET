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
    public async Task<Result<MovimientoDetailDto>> EnviarAPerdidas(RegistrarMovimientoRequest request, CancellationToken cancellationToken = default)
    {
        if (request.StoreId <= 0 || !request.ProductIds.Any())
        {
            return Result<MovimientoDetailDto>.Failure(
                Error.Validation("Product.Empty", "El ID de la tienda y las unidades perdidas son obligatorios.")
            );
        }

        // Buscar la bodega de pérdidas configurada para la tienda
        var bodegaPerdidas = await context.Warehouse
            .FirstOrDefaultAsync(s => s.StoreId == request.StoreId && s.Type == WarehouseType.LossWarehouse, cancellationToken); // Ajusta el enum si se llama diferente (ej. LossWarehouse, LostWarehouse, etc.)

        if (bodegaPerdidas == null)
        {
            return Result<MovimientoDetailDto>.Failure(
                Error.NotFound("Warehouse.NotFound", "No se encontró una bodega de pérdidas configurada para esta tienda.")
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

                // 2. Actualizar ubicación y estado de la unidad física a Perdida/Lost
                unidadReal.BodegaId = bodegaPerdidas.Id;
                unidadReal.Status = UnidadProductoStatus.Lost; 

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

            // Mapeo del DTO de respuesta
            var resultadoDto = new MovimientoDetailDto(
                MovimientoId: 0,
                BodegaOrigenId: bodegaOrigenId,
                NombreBodegaOrigen: nombreBodegaOrigen,
                BodegaDestinoId: (int)bodegaPerdidas.Id,
                NombreBodegaDestino: bodegaPerdidas.Name ?? bodegaPerdidas.Ubication,
                TipoMovimiento: TipoMovimiento.Perdida, // Asegúrate de tener este valor en tu enum de TipoMovimiento
                Cantidad: itemsDto.Count,
                CostoUnitario: 0,
                CostoTotal: 0,
                ReferenciaId: null,
                ReferenciaTipo: null,
                Lote: null,
                FechaVencimiento: null,
                Motivo: request.Observations ?? "Pérdida de inventario por auditoría",
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
                Error.Failure("Movimiento.Error", $"Error al procesar las pérdidas: {ex.Message}")
            );
        }
    }
}