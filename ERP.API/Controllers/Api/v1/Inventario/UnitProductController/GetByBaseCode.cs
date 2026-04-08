using ERP.DATA.Services.InventarioService.UnitProductService;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.UnitProductController;

public sealed class GetUnitProductByBaseCodeEndpoint(
    UnitProductService unitProductService,
    ILogger<GetUnitProductByIdEndpoint> logger
) : BaseGetEndpoint<GetUnitProductByCodeRequets,
    GetUnitProductByIdEndpoint, UnitProductDetailDto>(logger)
{
    [Tags("Inventario - UnitProducts")]
    [HttpGet(UnitProductEndpoints.GetByBaseCode, Name = "GetUnitProductByBaseCode")]
    public async override Task<ActionResult<UnitProductDetailDto>> HandleAsync(
        [FromQuery] GetUnitProductByCodeRequets request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult<UnitProductDetailDto>> GetEntity(
        GetUnitProductByCodeRequets request,
        CancellationToken cancellationToken)
    {
        var productos = await unitProductService.GetByBaseCode(request);

        if (productos.Count == 0)
            return NotFound();

        return Ok(productos);
    }
}