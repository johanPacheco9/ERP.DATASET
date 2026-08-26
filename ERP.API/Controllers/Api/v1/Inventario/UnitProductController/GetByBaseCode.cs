using ERP.DATA.Services.InventarioService.UnidadProductoService;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.UnitProductController;

public sealed class GetUnitProductByBaseCodeEndpoint(
    UnidadProductoManager unidadProductoManager,
    ILogger<GetUnitProductByIdEndpoint> logger
) : BaseGetEndpoint<GetUnitProductByCodeRequets,
    GetUnitProductByIdEndpoint, UnidadProductoDetailDto>(logger)
{
    [Tags("Inventario - UnitProducts")]
    [HttpGet(UnitProductEndpoints.GetByBaseCode, Name = "GetUnitProductByBaseCode")]
    public async override Task<ActionResult<UnidadProductoDetailDto>> HandleAsync(
        [FromQuery] GetUnitProductByCodeRequets request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult<UnidadProductoDetailDto>> GetEntity(
        GetUnitProductByCodeRequets request,
        CancellationToken cancellationToken)
    {
        var productos = await unidadProductoManager.GetByBaseCode(request);

        if (productos.Count == 0)
            return NotFound();

        return Ok(productos);
    }
}