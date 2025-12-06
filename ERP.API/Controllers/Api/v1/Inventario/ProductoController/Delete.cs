using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;


public sealed class DeleteProductoEndpoint(
    IProductoService productoService,
    ILogger<DeleteProductoEndpoint> logger)
    : BaseDeleteEndpoint<DeleteProveedorRequest, DeleteProductoEndpoint, IProductoService>(productoService, logger)
{
    [Tags("Inventario - Productos")]
    [HttpDelete(ProductosEndpoints.Get, Name = "DeleteProducto")]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] DeleteProveedorRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> DeleteEntity(
        DeleteProveedorRequest request,
        CancellationToken cancellationToken)
    {
        await Service.DeleteProductoById(request.Id, cancellationToken);

        TraceDeleted("Producto", request.Id);

        return Ok("Producto eliminado correctamente");
    }
}


