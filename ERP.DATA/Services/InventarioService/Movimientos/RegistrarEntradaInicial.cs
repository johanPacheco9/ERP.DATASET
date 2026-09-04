using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.Movimientos;

public partial class MovimientosManager
{
    /// <summary>
    /// Registra la entrada inicial de stock para una variante (recién creada o existente):
    /// crea las unidades físicas (UnidadProducto), el movimiento de Entrada y actualiza el
    /// stock de la bodega, todo en una sola transacción.
    ///
    /// Modo híbrido de seriales: si se proveen SerialesManual, deben coincidir en cantidad
    /// con Cantidad y se usan tal cual (para productos con serial de fábrica real). Si no se
    /// provee ninguno, el sistema autogenera un código interno secuencial por unidad.
    /// </summary>
    public async Task<Result<List<string>>> RegistrarEntradaInicial(
        RegistrarEntradaInicialRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Cantidad <= 0)
        {
            return Result<List<string>>.Failure(
                Error.Validation("EntradaInicial.CantidadInvalida", "La cantidad debe ser mayor a cero."));
        }

        if (request.SerialesManual is { Count: > 0 } && request.SerialesManual.Count != request.Cantidad)
        {
            return Result<List<string>>.Failure(
                Error.Validation("EntradaInicial.SerialesIncompletos",
                    $"Se indicaron {request.SerialesManual.Count} seriales pero la cantidad es {request.Cantidad}. Deben coincidir."));
        }

        var variante = await context.ProductoVariantes
            .FirstOrDefaultAsync(v => v.Id == request.ProductoVarianteId, cancellationToken);

        if (variante is null)
        {
            return Result<List<string>>.Failure(
                Error.NotFound("EntradaInicial.VarianteNoExiste", "La variante especificada no existe."));
        }

        var bodega = await context.Warehouse
            .FirstOrDefaultAsync(w => w.Id == request.BodegaId, cancellationToken);

        if (bodega is null)
        {
            return Result<List<string>>.Failure(
                Error.NotFound("EntradaInicial.BodegaNoExiste", "La bodega especificada no existe."));
        }

        // Si hay seriales manuales, validar que ninguno esté duplicado en el sistema
        if (request.SerialesManual is { Count: > 0 })
        {
            var duplicados = await context.UnidadesProductos
                .Where(u => request.SerialesManual.Contains(u.SerialNumber))
                .Select(u => u.SerialNumber)
                .ToListAsync(cancellationToken);

            if (duplicados.Count > 0)
            {
                return Result<List<string>>.Failure(
                    Error.Validation("EntradaInicial.SerialesDuplicados",
                        $"Los siguientes seriales ya existen en el sistema: {string.Join(", ", duplicados)}"));
            }
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var movement = new Movement
            {
                OrigenWarehouseId = request.BodegaId,
                DestinationWarehouseId = null,
                Type = TipoMovimiento.Entrada,
                Quantity = request.Cantidad,
                UnitCost = variante.CostoUnitario ?? 0,
                Observations = request.Observations,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request._CreatorAuth0Id
            };
            context.Movements.Add(movement);
            await context.SaveChangesAsync(cancellationToken);

            var serialesGenerados = new List<string>();

            // Punto de partida del correlativo autogenerado: unidades ya existentes de esta variante
            var siguienteCorrelativo = await context.UnidadesProductos
                .CountAsync(u => u.ProductoVarianteId == request.ProductoVarianteId, cancellationToken) + 1;

            for (int i = 0; i < request.Cantidad; i++)
            {
                var serial = request.SerialesManual is { Count: > 0 }
                    ? request.SerialesManual[i]
                    : $"{variante.SKU}-{(siguienteCorrelativo + i):D6}";

                serialesGenerados.Add(serial);

                var nuevaUnidad = new UnidadProducto
                {
                    BodegaId = request.BodegaId,
                    ProductoVarianteId = request.ProductoVarianteId,
                    SerialNumber = serial,
                    Status = UnidadProductoStatus.Available,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request._CreatorAuth0Id
                };
                context.UnidadesProductos.Add(nuevaUnidad);
                await context.SaveChangesAsync(cancellationToken);

                context.UnitProductMovements.Add(new UnitProductMovement
                {
                    UnidadProductoId = nuevaUnidad.Id,
                    MovimientoId = movement.Id,
                    TipoMovimiento = TipoMovimiento.Entrada,
                    BodegaOrigenId = request.BodegaId,
                    BodegaDestinoId = null,
                    Motivo = "Entrada inicial de stock",
                    Observaciones = request.Observations
                });
            }

            var stock = await context.WarehouseStock
                .FirstOrDefaultAsync(s => s.WarehouseId == request.BodegaId &&
                                           s.ProductoVarianteId == request.ProductoVarianteId, cancellationToken);

            if (stock != null)
            {
                stock.CurrentStock += request.Cantidad;
            }
            else
            {
                context.WarehouseStock.Add(new WarehouseStock
                {
                    WarehouseId = request.BodegaId,
                    ProductoVarianteId = request.ProductoVarianteId,
                    CurrentStock = request.Cantidad
                });
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<List<string>>.Success(serialesGenerados);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<List<string>>.Failure(
                Error.Failure("EntradaInicial.Failed", $"Error al registrar la entrada inicial: {ex.Message}"));
        }
    }
}

public class RegistrarEntradaInicialRequest
{
    public int ProductoVarianteId { get; set; }
    public int BodegaId { get; set; }
    public int Cantidad { get; set; }
    public List<string>? SerialesManual { get; set; }
    public string? Observations { get; set; }
    public int _CreatorAuth0Id { get; set; }
}