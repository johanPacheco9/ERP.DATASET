using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using Microsoft.AspNetCore.Mvc;
using ProductoBaseManager = ERP.DATA.Services.InventarioService.BaseProducto.ProductoBaseManager;


namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class GetProductoByIdEndpoint(
    ProductoBaseManager productoManager,
    ILogger<GetProductoByIdEndpoint> logger
) : BaseGetEndpoint<GetProductoByIdRequest, GetProductoByIdEndpoint, BaseProductDto>(logger)
{
    [Tags("Inventario - Productos")]

    [HttpGet(ProductEndpoints.Get, Name = ("GetProductoById"))]
    public async override Task<ActionResult<BaseProductDto>> HandleAsync(
        [FromRoute] GetProductoByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<BaseProductDto>> GetEntity(
      GetProductoByIdRequest request,
      CancellationToken cancellationToken)
    {
        var producto = await productoManager.GetProductoById(request.Id, cancellationToken);

        if (producto is null)
            return NotFound();
        TraceFound(nameof(ProductoBase), request.Id);

        return producto;
    }
}







