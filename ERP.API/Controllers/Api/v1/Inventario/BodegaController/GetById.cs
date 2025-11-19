using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class GetBodegaByIdEndpoint(IServiceProvider serviceProvider)
    : BaseGetEndpoint<GetBodegaByIdRequest, GetBodegaByIdEndpoint, BodegaDetailDTO>(serviceProvider)
{
    [Tags("Inventario - Bodegas")]
    [HttpGet(BodegasEndpoints.Get, Name = "GetBodegaByIdRequest")]
    public override async Task<ActionResult<BodegaDetailDTO>> HandleAsync(
        [FromRoute] GetBodegaByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<BodegaDetailDTO>> GetEntity(
        GetBodegaByIdRequest request,
        CancellationToken cancellationToken)
    {
        var bodegaService = serviceProvider.GetRequiredService<IBodegaService>();
        var bodega = await bodegaService.GetBodegaByIdAsync(request.Id, cancellationToken);

        if (bodega == null)
        {
            return NotFound();
        }

        var bodegaDto = new BodegaDetailDTO(
            bodega.Id,
            bodega.Nombre,
            bodega.Descripcion,
            bodega.Ubicacion,
            bodega.IsActive,
            bodega.CreatedAt,
            bodega.UpdatedAt
        );

        TraceFound(nameof(Bodega), bodega.Id);
        return Ok(bodegaDto);
    }
}