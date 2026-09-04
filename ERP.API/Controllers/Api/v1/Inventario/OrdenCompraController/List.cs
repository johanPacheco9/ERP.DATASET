using ERP.DATA.Services.InventarioService.OrdenesDeCompra;
using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.OrdenCompraController;

[ApiController]
public class ListOrdenesCompraEndpoint : ControllerBase
{
    private readonly OrdenesDeCompraManager _manager;

    public ListOrdenesCompraEndpoint(OrdenesDeCompraManager manager)
    {
        _manager = manager;
    }

    [Tags("Inventario - Órdenes de Compra")]
    [HttpGet(OrdenCompraEndpoints.List)]
    public async Task<ActionResult<List<OrdenCompraSummaryDto>>> HandleAsync(
        [FromQuery] OrdenCompraStatus? status,
        [FromQuery] int? proveedorId,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        CancellationToken cancellationToken)
    {
        var result = await _manager.GetList(status, proveedorId, desde, hasta, cancellationToken);
        return Ok(result);
    }
}
