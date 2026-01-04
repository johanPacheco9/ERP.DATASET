using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductosVariantes;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class CreateProductoEndpoint
    : BaseCreateEndpoint<CreateProductoRequest, CreateProductoEndpoint>
{
    private readonly IProductoService _productoService;
    private readonly IProductoVarianteService _productoVarianteService;

    public CreateProductoEndpoint(
        ILogger<CreateProductoEndpoint> logger,
        IProductoService productoService, IProductoVarianteService productoVarianteService)
        : base(logger)
    {
        _productoService = productoService;
        _productoVarianteService = productoVarianteService; 
    }

    [Tags("Inventario - Productos")]
    [HttpPost(ProductosEndpoints.List, Name = "CreateProducto")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateProductoRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(
        CreateProductoRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
        if (request.Variantes != null)
        {
            _productoVarianteService.AddProductoVariantes(request.Variantes, cancellationToken);
        }

        var productoId = await _productoService.AddProductoAsync(
            request,
            cancellationToken
        );

        TraceCreated("Producto", productoId);

        return CreatedAtRoute(
            "GetProductoById",
            new { id = productoId },
            new
            {
                id = productoId,
                message = "Producto creado exitosamente"
            }
        );
    }
}
