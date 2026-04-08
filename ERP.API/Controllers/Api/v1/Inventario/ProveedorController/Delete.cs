namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ISupplier;
using Microsoft.AspNetCore.Mvc;

public sealed class DeleteProveedorEndpoint(ISupplierService _proveedorService,ILogger<DeleteProductoEndpoint> logger)
    : BaseDeleteEndpoint<DeleteProveedorByIdRequest, DeleteProveedorEndpoint, ISupplierService>(_proveedorService)
{
    [Tags("Inventario - Proveedores")]
    [HttpDelete(ProveedorEndpoints.List, Name = ("Delete Supplier"))]
    public override async Task<ActionResult> HandleAsync(
        [FromRoute] DeleteProveedorByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> DeleteEntity(DeleteProveedorByIdRequest request, CancellationToken cancellationToken)
    {

        await _proveedorService.DeleteProveedorById(request.Id, cancellationToken);
        TraceDeleted("Supplier eliminado correctamente", request.Id);
        return Ok();

    }
}

