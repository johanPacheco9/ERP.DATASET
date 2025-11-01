namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;
using ERP.API.Controllers.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteProveedorEndpoint(IServiceProvider serviceProvider)
    : BaseDeleteEndpoint<DeleteProveedorByIdRequest, DeleteProveedorEndpoint>(serviceProvider)
{
    [Tags("Inventario - Proveedores")]
    [HttpDelete(ProveedorEndpoints.List, Name = ("Delete Proveedor"))]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] DeleteProveedorByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> DeleteEntity(DeleteProveedorByIdRequest request, CancellationToken cancellationToken)
    {
        await Repository.Proveedores
            .Where(p => p.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await Repository.SaveChangesAsync(cancellationToken);

        TraceDeleted("Producto eliminado correctamente",request.Id);

        return Ok();

    }
}

