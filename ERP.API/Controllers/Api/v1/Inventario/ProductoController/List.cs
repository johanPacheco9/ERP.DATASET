using ERP.API.Controllers.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

/// <summary>
///     Endpoint para listar productos con filtros y paginación.
/// </summary>
public sealed class List(IServiceProvider serviceProvider)
    : BaseListEndpoint<ListProductRequest, List, PagedList<ProductoSummaryDto>>(serviceProvider)
{
    [Tags("Inventario - Productos")]
    [HttpGet(ProductosEndpoints.List, Name = "List Productos")]
    public override async Task<ActionResult<PagedList<ProductoSummaryDto>>> HandleAsync(
        [FromQuery] ListProductRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    /// <summary>
    ///     Lógica para listar productos desde la base de datos.
    /// </summary>
    protected override async Task<ActionResult<PagedList<ProductoSummaryDto>>> ListEntity(ListProductRequest request,CancellationToken cancellationToken)
    {
        // 🔹 Base productos sin tracking
        var productos = Repository.Productos.AsNoTracking();



        // 🔹 Filtros opcionales
        if (request.MinDate.HasValue)
        {
            TraceListFiltered("Productos", "fecha mínima", request.MinDate.Value);
            productos = productos.Where(p => p.CreatedAt >= request.MinDate.Value);
        }

        if (request.MaxDate.HasValue)
        {
            TraceListFiltered("Productos", "fecha máxima", request.MaxDate.Value);
            productos = productos.Where(p => p.CreatedAt <= request.MaxDate.Value);
        }

        LogGeneratedQuery(productos);

        var results =
           await productos.PaginateAsync(l =>
                   new ProductoSummaryDto(l.Id,l.Codigo ,l.Nombre, l.Descripcion, l.Precio_Venta, 
                   l.Costo_Unitario, l.Unidad_Medida,l.Es_Perecedero,l.Categoria.Nombre,l.Proveedor.Nombre,l.Imagen_Url,l.Tags,l.IsActive),
               request.PageNumber, request.PageSize);


        PrepareResponseHeaders(results.PaginationHeaders);

        Logger.LogTrace(
            $"Retornando {results.Count} de {results.TotalCount}");
        return Ok(results);
    }
}
