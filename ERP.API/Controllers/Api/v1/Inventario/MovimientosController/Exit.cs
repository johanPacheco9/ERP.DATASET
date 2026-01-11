using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovimientos;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;

public sealed class CreateExitMovementEndpoint : BaseCreateEndpoint<CreateExitMovementRequest, CreateExitMovementEndpoint>
{
    private readonly IMovimientoService _movimientoService;
    public CreateExitMovementEndpoint(ILogger<CreateExitMovementEndpoint> logger, IMovimientoService movimientoService)
        : base(logger)
    {
        _movimientoService = movimientoService;
    }

    [Tags("Inventario - Movimientos")]
    [HttpPost(MovimientosEndpoints.ExitMovement)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateExitMovementRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(
     CreateExitMovementRequest request,
     CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }

        var resultado = await _movimientoService.RegistrarSalidaAsync(request, cancellationToken);

        if (resultado == null)
            return StatusCode(500, "Error registrando movimiento de salida.");

        return Ok(resultado);
    }
}
