using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.AspNetCore.Mvc;


namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class GetProductoByIdEndpoint : BaseGetEndpoint<GetProductoByIdRequest, GetProductoByIdEndpoint, ProductoBaseDto>
{
    private readonly IProductoService _productoService;
    public GetProductoByIdEndpoint(
      IProductoService productoService,
      ILogger<GetProductoByIdEndpoint> logger) : base(logger)
    {
        _productoService = productoService;
    }
    [Tags("Inventario - Productos")]

    [HttpGet(ProductosEndpoints.Get, Name = ("GetProductoById"))]
    public override async Task<ActionResult<ProductoBaseDto>> HandleAsync(
        [FromRoute] GetProductoByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<ProductoBaseDto>> GetEntity(
      GetProductoByIdRequest request,
      CancellationToken cancellationToken)
    {
        var producto = await _productoService.GetProductoById(request.Id, cancellationToken);

        if (producto is null)
            return NotFound();

        var productobaseDTO = new ProductoBaseDto
        (
            Id: producto.Id,
            Codigo: producto.Codigo,
            Nombre: producto.Nombre,
            Descripcion: producto.Descripcion,
            CategoriaId: producto.CategoriaId,
            ProveedorId: producto.ProveedorId,
            UnidadMedida: producto.Unidad_Medida,
            ImagenUrl: producto.Imagen_Url,
            Tags: producto.Tags,
            Activo: producto.IsActive
        );

        TraceFound(nameof(Producto), request.Id);

        return productobaseDTO;
    }
}







