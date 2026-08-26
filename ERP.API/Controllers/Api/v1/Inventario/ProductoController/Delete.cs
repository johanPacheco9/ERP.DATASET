using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using Microsoft.AspNetCore.Mvc;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;


public sealed class DeleteProductoEndpoint(
    ProductoBaseService productoService,
    ILogger<DeleteProductoEndpoint> logger)
    : BaseDeleteEndpoint<DeleteProveedorRequest, DeleteProductoEndpoint, ProductoBaseService>(productoService, logger)
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
        await Service.DeleteProductoById(request.Id, cancellationToken);

        TraceDeleted("Product", request.Id);

        return Ok("Product eliminado correctamente");
    }
}


