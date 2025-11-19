namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteProductoEndpoint(IServiceProvider serviceProvider)
    : BaseDeleteEndpoint<DeleteProveedorRequest, DeleteProductoEndpoint>(serviceProvider)
{
    [Tags("Inventario - Productos")]
    [HttpDelete(ProductosEndpoints.Get, Name = ("DeleteProducto"))]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] DeleteProveedorRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> DeleteEntity(DeleteProveedorRequest request, CancellationToken cancellationToken)
    {
        var productoService = serviceProvider.GetRequiredService<IProductoService>();
        await productoService.DeleteProductoById(request.Id, cancellationToken);
        TraceDeleted("Producto eliminado correctamente",request.Id);
        return Ok();

    }
}

