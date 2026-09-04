using ERP.DATA.Services.InventarioService.OrdenesDeCompra;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.OrdenCompraController;

public sealed class CreateOrdenCompraEndpoint(
    ILogger<CreateOrdenCompraEndpoint> logger,
    OrdenesDeCompraManager manager)
    : BaseCreateEndpoint<CreateOrdenCompraRequest, CreateOrdenCompraEndpoint>(logger)
{
    [Tags("Inventario - Órdenes de Compra")]
    [HttpPost(OrdenCompraEndpoints.Create)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateOrdenCompraRequest request,
        CancellationToken cancellationToken = new())
    {
        if (request == null)
            return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

        var result = await manager.Create(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtRoute(nameof(GetOrdenCompraByIdEndpoint), new { id = result.Value.Id }, result.Value);
    }

    protected override Task<ActionResult> CreateEntity(CreateOrdenCompraRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
