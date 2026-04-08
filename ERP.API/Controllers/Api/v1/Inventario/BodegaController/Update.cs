using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IWarehouse;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class PatchBodegaEndpoint
    : BaseUpdateEndpoint<UpdateWarehouseRequest, PatchBodegaEndpoint>
{
    private readonly IWarehouseService _bodegaService;

    public PatchBodegaEndpoint(
        ILogger<PatchBodegaEndpoint> logger,
        IWarehouseService bodegaService)
        : base(logger)
    {
        _bodegaService = bodegaService;
    }

    [Tags("Inventario - Bodegas")]
    [HttpPatch(BodegasEndpoints.List, Name = "PatchBodega")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] UpdateWarehouseRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> UpdateEntity(
        UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        var bodegaExistente = await _bodegaService.GetBodegaByIdAsync(
            request.Id,
            cancellationToken
        );

        if (bodegaExistente == null)
        {
            return EntityNotFound(nameof(Warehouse), request.Id);
        }

        await _bodegaService.UpdateBodega(request, cancellationToken);

        TraceUpdated(nameof(Warehouse), request.Id);

        return Ok(new
        {
            id = request.Id,
            message = "Warehouse actualizada parcialmente exitosamente"
        });
    }
}