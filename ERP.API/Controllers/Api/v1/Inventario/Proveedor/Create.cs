using ERP.API.Controllers.Utilities.Base;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
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


        var proveedor = new Proveedor
        {
            Nombre = request.Nombre,
            Nit = request.Nit,
            Direccion = request.Direccion,
            Telefono = request.Telefono,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "01",
            IsActive = true
        }; 

        await Repository.Proveedores.AddAsync(proveedor);

        await Repository.SaveChangesAsync(cancellationToken);

        return Created();
    }
}