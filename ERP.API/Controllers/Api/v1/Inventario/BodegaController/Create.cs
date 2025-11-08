using ERP.API.Controllers.Utilities.Base;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using Microsoft.AspNetCore.Mvc;

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
        // 1. Validar el request
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }

        // 2. MAPEAR Request DTO → Entidad de Dominio
        var bodega = new Bodega
        {
            // Asumo las propiedades de tu entidad Bodega
            Nombre = request.Nombre,
            Codigo = request.Code, // Si la entidad usa "Codigo" y el request "Code"
            Descripcion = request.Descripcion,
            Ubicacion = request.Ubicacion,
            Capacidad_Maxima = request.CapacidadMaxima,
            IsActive = request.EsActiva,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = "01"
        };

        var bodegaService = serviceProvider.GetRequiredService<IBodegaService>();
        var bodegaId = await bodegaService.AddBodegaAsync(bodega, cancellationToken);

        // 4. Retornar respuesta
        return CreatedAtRoute("GetBodegaById", new { id = bodegaId }, new
        {
            id = bodegaId,
            nombre = bodega.Nombre,
            codigo = bodega.Codigo,
            message = "Bodega creada exitosamente"
        });
    }
}