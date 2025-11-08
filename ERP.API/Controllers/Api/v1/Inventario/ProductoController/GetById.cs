using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class GetProductoByIdEndpoint(IServiceProvider serviceProvider)
    : BaseGetEndpoint<GetProductoByIdRequest, GetProductoByIdEndpoint, ProductoBaseDto>(serviceProvider)
{

    [Tags("Inventario - Productos")]

    [HttpGet(ProductosEndpoints.Get, Name = ("GetProductoById"))]
    public override async Task<ActionResult<ProductoBaseDto>> HandleAsync(
        [FromRoute] GetProductoByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<ProductoBaseDto>> GetEntity(GetProductoByIdRequest request, CancellationToken cancellationToken)
    {
        var productoService = HttpContext.RequestServices.GetRequiredService<IProductoService>();
        var response = productoService.GetProductoById(request.Id, cancellationToken);
        var producto = response.Result;

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







