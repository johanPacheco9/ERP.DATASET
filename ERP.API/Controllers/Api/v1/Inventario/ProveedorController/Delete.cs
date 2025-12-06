namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores;
using Microsoft.AspNetCore.Mvc;

public sealed class DeleteProveedorEndpoint(IProveedorService _proveedorService,ILogger<DeleteProductoEndpoint> logger)
    : BaseDeleteEndpoint<DeleteProveedorByIdRequest, DeleteProveedorEndpoint, IProveedorService>(_proveedorService)
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

        await _proveedorService.DeleteProveedorById(request.Id, cancellationToken);
        TraceDeleted("Proveedor eliminado correctamente", request.Id);
        return Ok();

    }
}

