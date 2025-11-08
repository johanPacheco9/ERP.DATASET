using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProveedorController;

public sealed class CreateProductoEndpoint(IServiceProvider serviceProvider)
    : BaseCreateEndpoint<CreateProveedorRequest, CreateProductoEndpoint>(serviceProvider)
{
    [Tags("Inventario - Proveedores")]
    [HttpPost(ProveedorEndpoints.List, Name = ("Crear Proveedor"))]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateProveedorRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateProveedorRequest request, CancellationToken cancellationToken)
    {
        var proveedorService = serviceProvider.GetRequiredService<IProveedorService>();

        await proveedorService.AddProveedorAsync(request, cancellationToken);

        return Created();
    }

}