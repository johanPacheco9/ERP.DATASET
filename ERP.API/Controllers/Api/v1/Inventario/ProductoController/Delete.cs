namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;
using ERP.API.Controllers.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
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
        await Repository.Productos
            .Where(p => p.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await Repository.SaveChangesAsync(cancellationToken);

        TraceDeleted("Producto eliminado correctamente",request.Id);

        return Ok();

    }
}

