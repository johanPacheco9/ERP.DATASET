using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IWarehouse;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class CreateBodegaEndpoint : BaseCreateEndpoint<CreateBodegaRequest, CreateBodegaEndpoint>
{
    private readonly IWarehouseService _warehouseService;

    public CreateBodegaEndpoint(ILogger<CreateBodegaEndpoint> logger, IWarehouseService warehouseService)
       : base(logger)
    {
        _warehouseService= warehouseService;
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
        var bodegaId = await _warehouseService.AddBodegaAsync(request, cancellationToken);

        // 4. Retornar respuesta
        return CreatedAtRoute("GetBodegaById", new { id = bodegaId }, new
        {
            id = bodegaId,
            message = "Warehouse creada exitosamente"
        });
    }
}