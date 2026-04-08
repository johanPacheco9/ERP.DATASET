using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;
namespace ERP.API.Controllers.Api.v1.Inventario.MovimientosController;

public sealed class List(
    ILogger<List> logger,
    MovimientoService movimientoService
) : BaseListEndpoint<ListMovementsRequest, List, PagedList<MovimientoDetailDto>>(logger)
{
    [Tags("Inventario - Movimientos")]
    [HttpGet("list", Name = "List Movimientos")]
    public async override Task<ActionResult<PagedList<MovimientoDetailDto>>> HandleAsync(
        [FromQuery] ListMovementsRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    /// <summary>
    /// Lógica para obtener y paginar los movimientos desde el servicio.
    /// </summary>
    protected async override Task<ActionResult<PagedList<MovimientoDetailDto>>> ListEntity(
        ListMovementsRequest request,
        CancellationToken cancellationToken
    )
    { 
        var response = await movimientoService.ListMovements(request);
        return Ok(response);
    }
}