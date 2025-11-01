using ERP.API.Controllers.Utilities.Base;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;
public sealed class CreateEntradaEndpoint(IServiceProvider serviceProvider)
    : BaseCreateEndpoint<RegistrarMovimientoEntradaRequest, CreateEntradaEndpoint>(serviceProvider)
{
    [Tags("Inventario - Movimientos")]
    [HttpPost(MovimientosEndpoints.List)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] RegistrarMovimientoEntradaRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(RegistrarMovimientoEntradaRequest request, CancellationToken cancellationToken)
    {
        // 1. Buscar si ya existe stock para este producto-bodega
        var stockExistente = await Repository.StockBodegas
            .FirstOrDefaultAsync(bs => bs.BodegaId == request.BodegaId &&
                                    bs.ProductoVarianteId == request.ProductoId, cancellationToken);

        // 2. Si NO existe, crear nuevo registro de stock
        if (stockExistente == null)
        {
            var nuevoStock = new StockBodega
            {
                Id = Guid.NewGuid(),
                BodegaId = request.BodegaId,
                ProductoVarianteId = request.ProductoId,
                StockActual = request.Cantidad,
                StockMinimo = 0, // Valor por defecto
                StockMaximo = 0, // Valor por defecto  
                StockReservado = 0,
                FechaActualizacion = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "1",
                IsActive = true,
            };
            Repository.StockBodegas.Add(nuevoStock);
        }
        else
        {
            // 3. Si EXISTE, ACTUALIZAR el registro existente
            stockExistente.StockActual += request.Cantidad;
            stockExistente.FechaActualizacion = DateTime.UtcNow;
            stockExistente.UpdatedAt = DateTime.UtcNow;
            stockExistente.UpdatedBy = "1";
        }

        // 4. Crear el movimiento
        var movimiento = new Movimiento
        {
            Id = Guid.NewGuid(),
            ProductoVarianteId = request.ProductoId,
            BodegaId = request.BodegaId,
            TipoMovimiento = TipoMovimiento.Entrada,
            Cantidad = request.Cantidad,
            CostoUnitario = request.CostoUnitario,
            Lote = request.Lote,
            FechaVencimiento = request.FechaVencimiento,
            Motivo = request.Motivo,
            Observaciones = request.Observaciones,
            ReferenciaId = request.ReferenciaId,
            ReferenciaTipo = request.ReferenciaTipo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "1",
            IsActive = true,
        };

        Repository.Movimientos.Add(movimiento);

        // 5. Guardar ambos cambios
        await Repository.SaveChangesAsync(cancellationToken);

        return StatusCode(StatusCodes.Status201Created);
    }
}
 


