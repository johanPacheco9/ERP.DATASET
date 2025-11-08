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
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
        var proveedor = new Proveedor
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Nit = request.Nit,
            Direccion = request.Direccion,
            Telefono = request.Telefono,
            Activo = request.Activo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system"
        };
        var proveedorService = serviceProvider.GetRequiredService<IProveedorService>();
        var proveedorCreado = await proveedorService.AddProveedorAsync(proveedor, cancellationToken);
        return CreatedAtRoute("GetProveedorById", new { id = proveedorCreado.Id }, new
        {
            id = proveedorCreado.Id,
            nombre = proveedorCreado.Nombre,
            message = "Proveedor creado exitosamente"
        });
    }

}