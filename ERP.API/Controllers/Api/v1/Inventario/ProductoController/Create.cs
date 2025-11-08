using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class CreateProductoEndpoint(IServiceProvider serviceProvider)
    : BaseCreateEndpoint<CreateProductoRequest, CreateProductoEndpoint>(serviceProvider)
{
    [Tags("Inventario - Productos")]
    [HttpPost(ProductosEndpoints.List, Name = "Create Producto")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateProductoRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateProductoRequest request, CancellationToken cancellationToken)
    {
        var productoService = HttpContext.RequestServices.GetRequiredService<IProductoService>();

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            // otrass propiedades..
        };

        var result = await productoService.AddProductoAsync(producto, cancellationToken);

        return CreatedAtAction(nameof(HandleAsync), new { id = result.Id }, result);
    }
}
