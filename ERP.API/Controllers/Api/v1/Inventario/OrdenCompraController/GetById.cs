using ERP.DATA.Services.InventarioService.OrdenesDeCompra;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.OrdenCompraController;

[ApiController]
public class GetOrdenCompraByIdEndpoint : ControllerBase
{
    private readonly OrdenesDeCompraManager _manager;

    public GetOrdenCompraByIdEndpoint(OrdenesDeCompraManager manager)
    {
        _manager = manager;
    }

    [Tags("Inventario - Órdenes de Compra")]
    [HttpGet(OrdenCompraEndpoints.GetById, Name = nameof(GetOrdenCompraByIdEndpoint))]
    public async Task<ActionResult<OrdenCompraDetailDto>> HandleAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await _manager.GetById(id, cancellationToken);
        if (result == null)
            return NotFound(new { error = $"Orden de compra #{id} no encontrada." });

        return Ok(result);
    }
}
