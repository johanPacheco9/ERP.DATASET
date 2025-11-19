using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovimientos;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;

public sealed class CreateEntradaEndpoint : BaseCreateEndpoint<RegistrarMovimientoEntradaRequest, CreateEntradaEndpoint>
{
    private readonly IMovimientoService _movimientoService;

    
    public CreateEntradaEndpoint(ILogger<CreateEntradaEndpoint> logger, IMovimientoService movimientoService)
        : base(logger)
    {
        _movimientoService = movimientoService;
    }

    [Tags("Inventario - Movimientos")]
    [HttpPost(MovimientosEndpoints.List)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] RegistrarMovimientoEntradaRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(
     RegistrarMovimientoEntradaRequest request,
     CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
        var movimiento = new Movimiento
        {
            Id = Guid.NewGuid(),
            ProductoId = request.ProductoId,
            BodegaId = request.BodegaId,
            TipoMovimiento = TipoMovimiento.Entrada,

            Cantidad = request.Cantidad,
            ReferenciaId = request.ReferenciaId,
            ReferenciaTipo = request.ReferenciaTipo,

            Lote = request.Lote,
            FechaVencimiento = request.FechaVencimiento,

            Motivo = request.Motivo,
            Observaciones = request.Observaciones,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = "01"
        };
        var resultado = await _movimientoService.RegistrarEntradaAsync(movimiento, cancellationToken);

        if (!resultado)
            return StatusCode(500, "Error registrando movimiento de entrada.");

        // Aquí podrías retornar el movimiento guardado
        return CreatedAtRoute("GetMovimientoById", new { id = movimiento.Id }, new
        {
            id = movimiento.Id,
            tipo = "Entrada",
            message = "Movimiento de entrada registrado exitosamente"
        });
    }

}