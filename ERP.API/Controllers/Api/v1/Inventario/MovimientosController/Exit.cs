using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovement;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;

public sealed class CreateExitMovementEndpoint(ILogger<CreateExitMovementEndpoint> logger, MovimientoService movimientoService)
    : BaseCreateEndpoint<CreateExitMovementRequest, CreateExitMovementEndpoint>(logger)
{
    [Tags("Inventario - Movimientos")]
    [HttpPost(MovimientosEndpoints.ExitMovement)]
    public async override Task<ActionResult> HandleAsync(
        [FromBody] CreateExitMovementRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult> CreateEntity(
     CreateExitMovementRequest request,
     CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }

        var resultado = await movimientoService.RegistrarSalidaAsync(request, cancellationToken);

        return Ok(resultado);
    }
}
