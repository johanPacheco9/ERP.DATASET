using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class PatchBodegaEndpoint
    : BaseUpdateEndpoint<UpdateBodegaRequest, PatchBodegaEndpoint>
{
    private readonly IBodegaService _bodegaService;

    public PatchBodegaEndpoint(
        ILogger<PatchBodegaEndpoint> logger,
        IBodegaService bodegaService)
        : base(logger)
    {
        _bodegaService = bodegaService;
    }

    [Tags("Inventario - Bodegas")]
    [HttpPatch(BodegasEndpoints.List, Name = "PatchBodega")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] UpdateBodegaRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> UpdateEntity(
        UpdateBodegaRequest request,
        CancellationToken cancellationToken)
    {
        var bodegaExistente = await _bodegaService.GetBodegaByIdAsync(
            request.Id,
            cancellationToken
        );

        if (bodegaExistente == null)
        {
            return EntityNotFound(nameof(Bodega), request.Id);
        }

        await _bodegaService.UpdateBodega(request, cancellationToken);

        TraceUpdated(nameof(Bodega), request.Id);

        return Ok(new
        {
            id = request.Id,
            message = "Bodega actualizada parcialmente exitosamente"
        });
    }
}