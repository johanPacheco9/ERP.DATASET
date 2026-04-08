using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovement;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;

public sealed class CreateEntradaEndpoint(ILogger<CreateEntradaEndpoint> logger, MovimientoService movimientoService)
    : BaseCreateEndpoint<CreateEntryMovementRequest, CreateEntradaEndpoint>(logger)
{
    [Tags("Inventario - Movimientos")]
    [HttpPost(MovimientosEndpoints.List)]
    public async override Task<ActionResult> HandleAsync(
        [FromBody] CreateEntryMovementRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult> CreateEntity(
     CreateEntryMovementRequest request,
     CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
       
        var resultado = await movimientoService.RegistrarEntradaAsync(request, cancellationToken);

        if (resultado == -1)
            return StatusCode(500, "Error registrando movimiento de entrada.");

        return CreatedAtRoute("GetMovimientoById", new { id = resultado }, new
        {
            resultado,
            tipo = "Entrada",
            message = "Movement de entrada registrado exitosamente"
        });
    }

}