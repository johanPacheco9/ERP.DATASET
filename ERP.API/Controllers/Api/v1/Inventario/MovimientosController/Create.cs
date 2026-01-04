using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovimientos;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;

public sealed class CreateEntradaEndpoint : BaseCreateEndpoint<CreateEntryMovementRequest, CreateEntradaEndpoint>
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
        [FromBody] CreateEntryMovementRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(
     CreateEntryMovementRequest request,
     CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
       
        var resultado = await _movimientoService.RegistrarEntradaAsync(request, cancellationToken);

        if (resultado == null || resultado == -1)
            return StatusCode(500, "Error registrando movimiento de entrada.");

        return CreatedAtRoute("GetMovimientoById", new { id = resultado }, new
        {
            resultado,
            tipo = "Entrada",
            message = "Movimiento de entrada registrado exitosamente"
        });
    }

}