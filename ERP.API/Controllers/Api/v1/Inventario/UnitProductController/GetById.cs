using ERP.DATA.Services.InventarioService.UnidadProductoService;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.UnitProductController;

public sealed class GetUnitProductByIdEndpoint(
    UnidadProductoManager unidadProductoManager,
    ILogger<GetUnitProductByIdEndpoint> logger
) : BaseGetEndpoint<GetByIdRequest, GetUnitProductByIdEndpoint, UnidadProductoDetailDto>(logger)
{
    [Tags("Inventario -UnitProducts")]

    [HttpGet(UnitProductEndpoints.Get, Name = ("GetUnitProductById"))]
    public override async Task<ActionResult<UnidadProductoDetailDto>> HandleAsync(
        [FromRoute] GetByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult<UnidadProductoDetailDto>> GetEntity(
      GetByIdRequest request,
      CancellationToken cancellationToken)
    {
        var producto = await unidadProductoManager.GetById(request.Id, cancellationToken);

        if (producto is null)
            return NotFound();
        TraceFound(nameof(ProductoBase), request.Id);

        return producto;
    }
}







