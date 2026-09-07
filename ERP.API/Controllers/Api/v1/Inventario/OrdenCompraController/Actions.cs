using ERP.DATA.Services.InventarioService.OrdenesDeCompra;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.OrdenCompraController;

[ApiController]
public class OrdenCompraActionsEndpoint : ControllerBase
{
    private readonly OrdenesDeCompraManager _manager;

    // TODO: reemplazar por el Id del usuario autenticado (JWT/Auth0) cuando se conecte
    // la autenticación real. Por ahora se usa un Id de placeholder para que el tipo
    // coincida con lo que espera OrdenesDeCompraManager (int, no el nombre en texto).
    private const int PlaceholderUserId = 1;

    public OrdenCompraActionsEndpoint(OrdenesDeCompraManager manager)
    {
        _manager = manager;
    }

    [Tags("Inventario - Órdenes de Compra")]
    [HttpPut(OrdenCompraEndpoints.Aprobar)]
    public async Task<ActionResult> Aprobar(
        [FromRoute] int id,
        [FromBody] AproveOrdenCompraRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _manager.Aprobar(id, PlaceholderUserId, request.Observaciones, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [Tags("Inventario - Órdenes de Compra")]
    [HttpPut(OrdenCompraEndpoints.Enviar)]
    public async Task<ActionResult> Enviar(
        [FromRoute] int id,
        [FromBody] EnviarOrdenCompraRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _manager.Enviar(id, PlaceholderUserId, request.Observaciones, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [Tags("Inventario - Órdenes de Compra")]
    [HttpPut(OrdenCompraEndpoints.Cancelar)]
    public async Task<ActionResult> Cancelar(
        [FromRoute] int id,
        [FromBody] CancelarOrdenCompraRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _manager.Cancelar(id, PlaceholderUserId, request.Motivo, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok();
    }
}

public class EnviarOrdenCompraRequest
{
    public string? Observaciones { get; set; }
}

public class CancelarOrdenCompraRequest
{
    public string Motivo { get; set; } = string.Empty;
}