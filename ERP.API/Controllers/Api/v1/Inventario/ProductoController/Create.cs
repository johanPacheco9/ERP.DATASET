using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class CreateProductoEndpoint : BaseCreateEndpoint<CreateProductoRequest, CreateProductoEndpoint>
{
    private readonly IProductoService _productoService;
    public CreateProductoEndpoint(ILogger<CreateProductoEndpoint> logger, IProductoService productoService)
        : base(logger)
    {
        _productoService = productoService;
    }

    [Tags("Inventario - Productos")]
    [HttpPost(ProductosEndpoints.List, Name = "CreateProducto")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateProductoRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateProductoRequest request, CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Costo_Unitario = request.Costo_Unitario,
            Precio_Venta = request.Precio_Venta,
            CategoriaId = request.CategoriaId,
            ProveedorId = request.ProveedorId,
            Unidad_Medida = request.Unidad_Medida,
            CreatedAt = DateTime.UtcNow,
        };

        var productoCreado = await _productoService.AddProductoAsync(producto, cancellationToken);

        TraceCreated(nameof(Producto), productoCreado.Id);

        return CreatedAtRoute("GetProductoById", new { id = productoCreado.Id }, new
        {
            id = productoCreado.Id,
            codigo = productoCreado.Codigo,
            nombre = productoCreado.Nombre,
            message = "Producto creado exitosamente"
        });
    }
}