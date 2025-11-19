namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores;
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
        var productoService = serviceProvider.GetRequiredService<IProveedorService>();
        await productoService.DeleteProveedorById(request.Id, cancellationToken);
        TraceDeleted("Proveedor eliminado correctamente", request.Id);
        return Ok();

    }
}

