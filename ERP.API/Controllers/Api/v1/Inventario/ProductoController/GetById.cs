using ERP.API.Controllers.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class GetProductoByIdEndpoint(IServiceProvider serviceProvider)
    : BaseGetEndpoint<GetProveedorByIdRequest, GetProductoByIdEndpoint, ProductoBaseDto>(serviceProvider)
{

    [Tags("Inventario - Productos")]
    
    [HttpGet(ProductosEndpoints.Get, Name = ("GetProductoById"))]
    public override async Task<ActionResult<ProductoBaseDto>> HandleAsync(
        [FromRoute] GetProveedorByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<ProductoBaseDto>> GetEntity(GetProveedorByIdRequest request, CancellationToken cancellationToken)
    {

        var producto = await Repository.Productos.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (producto is null)
            return EntityNotFound(nameof(Producto));


        var productoDto = new ProductoBaseDto
        (
            Id : producto.Id,
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

        // Registrar que se encontró correctamente
        TraceFound(nameof(Producto), request.Id);

        return productoDto;
    }
}







