using ERP.DATA.Services.InventarioService.UnitProductService;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.UnitProductController;

public sealed class GetUnitProductByIdEndpoint(
    UnitProductService unitProductService,
    ILogger<GetUnitProductByIdEndpoint> logger
) : BaseGetEndpoint<GetByIdRequest, GetUnitProductByIdEndpoint, UnitProductDetailDto>(logger)
{
    [Tags("Inventario -UnitProducts")]

    [HttpGet(UnitProductEndpoints.Get, Name = ("GetUnitProductById"))]
    public override async Task<ActionResult<UnitProductDetailDto>> HandleAsync(
        [FromRoute] GetByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult<UnitProductDetailDto>> GetEntity(
      GetByIdRequest request,
      CancellationToken cancellationToken)
    {
        var producto = await unitProductService.GetById(request.Id, cancellationToken);

        if (producto is null)
            return NotFound();
        TraceFound(nameof(Product), request.Id);

        return producto;
    }
}







