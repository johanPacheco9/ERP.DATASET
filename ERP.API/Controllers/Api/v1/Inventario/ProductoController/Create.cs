using ERP.DATA.Services.InventarioService.ProductService;
using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductVariant;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class CreateProductoEndpoint(
    ILogger<CreateProductoEndpoint> logger,
    ProductService productoService, IProductVariantService productoVarianteService
)
    : BaseCreateEndpoint<CreateProductoRequest, CreateProductoEndpoint>(logger)
{
    [Tags("Inventario - Productos")]
    [HttpPost(ProductEndpoints.List, Name = "CreateProducto")]
    public async override Task<ActionResult> HandleAsync(
        [FromBody] CreateProductoRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected async override Task<ActionResult> CreateEntity(
        CreateProductoRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
        if (request.Variantes != null)
        {
            await productoVarianteService.AddProductoVariantes(request.Variantes, cancellationToken);
        }

        var productoId = await productoService.AddProductoAsync(
            request,
            cancellationToken
        );

        TraceCreated("Product", productoId);

        return CreatedAtRoute(
            "GetProductoById",
            new { id = productoId },
            new
            {
                id = productoId,
                message = "Product creado exitosamente"
            }
        );
    }
}
