using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using Microsoft.AspNetCore.Mvc;
using ProductoBaseManager = ERP.DATA.Services.InventarioService.BaseProducto.ProductoBaseManager;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;


public sealed class DeleteProductoEndpoint(
    ProductoBaseManager productoManager,
    ILogger<DeleteProductoEndpoint> logger)
    : BaseDeleteEndpoint<DeleteProveedorRequest, DeleteProductoEndpoint, ProductoBaseManager>(productoManager, logger)
{
    [Tags("Inventario - Productos")]
    [HttpDelete(ProductEndpoints.Get, Name = "DeleteProducto")]
    public async override Task<ActionResult> HandleAsync(
        [FromRoute] DeleteProveedorRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult> DeleteEntity(
        DeleteProveedorRequest request,
        CancellationToken cancellationToken)
    {
        await Manager.DeleteProductoById(request.Id, cancellationToken);

        TraceDeleted("Product", request.Id);

        return Ok("Product eliminado correctamente");
    }
}


