using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class CreateBodegaEndpoint : BaseCreateEndpoint<CreateBodegaRequest, CreateBodegaEndpoint>
{
    private readonly IBodegaService _bodegaService;

    public CreateBodegaEndpoint(ILogger<CreateBodegaEndpoint> logger, IBodegaService bodegaService)
       : base(logger)
    {
        _bodegaService= bodegaService;
    }

    [Tags("Inventario - Bodegas")]
    [HttpPost(BodegasEndpoints.List)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateBodegaRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateBodegaRequest request, CancellationToken cancellationToken)
    {
     
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
        var bodegaId = await _bodegaService.AddBodegaAsync(request, cancellationToken);

        // 4. Retornar respuesta
        return CreatedAtRoute("GetBodegaById", new { id = bodegaId }, new
        {
            id = bodegaId,
            message = "Bodega creada exitosamente"
        });
    }
}