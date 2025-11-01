using ERP.API.Controllers.Utilities.Base;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

public sealed class CreateProductoEndpoint(IServiceProvider serviceProvider)
    : BaseCreateEndpoint<CreateProductoRequest, CreateProductoEndpoint>(serviceProvider)
{
    [Tags("Inventario - Productos")]
    [HttpPost(ProductosEndpoints.List, Name = ("Create Producto"))]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateProductoRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateProductoRequest request, CancellationToken cancellationToken)
    {
        var codigo = $"PRD-{request.Codigo[..3].ToUpper()}";

        var exists = await Repository.Productos.AnyAsync(c => c.Codigo == codigo, cancellationToken);
        if (exists)
            return Conflict($"Ya existe una Bodega con el código '{codigo}'.");

        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            Codigo = codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Costo_Unitario = request.Costo_Unitario,
            Precio_Venta = request.Precio_Venta,
            PorcentajeIVA = request.PorcentajeIVA,
            PorcentajeICA = request.PorcentajeICA,
            ImpuestoEspecifico = request.ImpuestoEspecifico,
            ArancelImportacion = request.ArancelImportacion,
            ExentoIVA = request.ExentoIVA,
            GravadoICA = request.GravadoICA,
            CodigoTributario = request.CodigoTributario,
            CategoriaId = request.CategoriaId,
            ProveedorId = request.ProveedorId,
            Unidad_Medida = request.Unidad_Medida,
            Peso = request.Peso,
            Volumen = request.Volumen,
            Dimensiones = request.Dimensiones,
            Imagen_Url = request.Imagen_Url,
            Notas = request.Notas,
            Tags = request.Tags,
            CreatedAt = DateTime.UtcNow,
            CreatedBy  = "01",
            IsActive = true
        };

        Repository.Productos.Add(producto);

        await Repository.SaveChangesAsync(cancellationToken);

        return Created();
    }
}