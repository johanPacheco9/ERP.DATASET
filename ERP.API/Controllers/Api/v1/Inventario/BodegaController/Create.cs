using ERP.API.Controllers.Utilities.Base;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers.Api.v1.Inventario.BodegaController;

public sealed class CreateBodegaEndpoint(IServiceProvider serviceProvider)
    : BaseCreateEndpoint<CreateBodegaRequest, CreateBodegaEndpoint>(serviceProvider)
{
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
        var codigo = $"BOD-{request.Code[..3].ToUpper()}";

        var exists = await Repository.Bodegas.AnyAsync(c => c.Codigo == codigo, cancellationToken);
        if (exists)
            return Conflict($"Ya existe una Bodega con el código '{codigo}'.");

        var bodega = new Bodega
        {
            Id = Guid.NewGuid(),
            Codigo = codigo,
            Nombre = request.Nombre,
            Ubicacion = request.Ubicacion,
            Descripcion = request.Descripcion,
            CreatedBy = "1",
            CreatedAt = DateTime.UtcNow,
            Capacidad_Maxima = request.CapacidadMaxima,
            IsActive = true,
            UpdatedAt = null,
            UpdatedBy = null,
            
        };

        Repository.Bodegas.Add(bodega);

        await Repository.SaveChangesAsync(cancellationToken);

        return CreatedAtRoute("GetBodegaByIdRequest", new { id = bodega.Id }, bodega);
    }
}

