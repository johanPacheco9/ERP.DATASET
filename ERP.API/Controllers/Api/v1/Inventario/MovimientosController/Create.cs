using ERP.DATA.Services.InventarioService.Movimientos;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;

public sealed class CreateEntradaEndpoint(ILogger<CreateEntradaEndpoint> logger, MovimientosManager movimientosManager)
    : BaseCreateEndpoint<RegistrarMovimientoEntradaRequest, CreateEntradaEndpoint>(logger)
{
    [Tags("Inventario - Movimientos")]
    [HttpPost(MovimientosEndpoints.List)]
    public async override Task<ActionResult> HandleAsync(
        [FromBody] RegistrarMovimientoEntradaRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult> CreateEntity(
        RegistrarMovimientoEntradaRequest request,
     CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
       
        var resultado = await movimientosManager.Registrar(request, cancellationToken);

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