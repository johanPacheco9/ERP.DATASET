using ERP.API.Controllers.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class GetBodegaByIdEndpoint(IServiceProvider serviceProvider)
    : BaseGetEndpoint<GetBodegaByIdRequest, GetBodegaByIdEndpoint, BodegaDetailDTO>(serviceProvider)
{


    //[Authorize]
    [Tags("Inventario - Bodegas")]
    [HttpGet(BodegasEndpoints.Get, Name =("GetBodegaByIdRequest"))]
    public override async Task<ActionResult<BodegaDetailDTO>> HandleAsync(
        [FromRoute] GetBodegaByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<BodegaDetailDTO>> GetEntity(GetBodegaByIdRequest request, CancellationToken cancellationToken)
    {
        
        var bodega = await Repository.Bodegas.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (bodega is null)
            return EntityNotFound(nameof(Bodega));

        // Mapear la entidad a DTO
        var bodegaDto = new BodegaDetailDTO
        (
            Id : bodega.Id,
            Nombre : bodega.Nombre,
            Descripcion : bodega.Descripcion,
            Ubicacion : bodega.Ubicacion,
            FechaCreacion : bodega.CreatedAt,
            FechaModificacion : bodega.UpdatedAt,
            Activa : bodega.IsActive
        );

        // Registrar que se encontró correctamente
        TraceFound(nameof(Bodega), request.Id);

        return Ok(bodegaDto);
    }
}







