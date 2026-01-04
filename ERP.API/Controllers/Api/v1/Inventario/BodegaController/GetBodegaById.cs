using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class GetBodegaByIdEndpoint : BaseGetEndpoint<GetBodegaByIdRequest, GetBodegaByIdEndpoint, BodegaDetailDTO>
{
    private readonly IBodegaService _bodegaService;
    public GetBodegaByIdEndpoint(IBodegaService bodegaService, ILogger<GetBodegaByIdEndpoint> logger) : base(logger)
    {
        _bodegaService = bodegaService;
    }

    [Tags("Inventario - Bodegas")]
    [HttpGet(BodegasEndpoints.Get, Name = "GetBodegaById")]
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
        
        var bodega = await _bodegaService.GetBodegaByIdAsync(request.Id, cancellationToken);

        if (bodega == null)
        {
            return NotFound();
        }

        var bodegaDto = new BodegaDetailDTO(
            bodega.Id,
            bodega.Nombre,
            bodega.Descripcion,
            bodega.Ubicacion,
            bodega.Activa,
            bodega.FechaCreacion,
            bodega.FechaModificacion
        );

        TraceFound(nameof(Bodega), bodega.Id);
        return Ok(bodegaDto);
    }
}