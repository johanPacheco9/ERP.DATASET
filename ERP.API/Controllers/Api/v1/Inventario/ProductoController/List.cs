using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

/// <summary>
///     Endpoint para listar productos con filtros y paginación.
/// </summary>
public sealed class List 
    : BaseListEndpoint<ListProductRequest, List, PagedList<ProductoSummaryDto>>
{
    private readonly IProductoService _productoService;

    public List(ILogger<List> logger, IProductoService productoService)
        : base(logger)
    {
        _productoService = productoService;
    }

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
    protected override async Task<ActionResult<PagedList<ProductoSummaryDto>>> ListEntity(
        ListProductRequest request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ Llamas al servicio que devuelve entidades del dominio
        var productos = await _productoService.ListAsync(request, cancellationToken);

        System.Console.WriteLine(productos);

        // 2️⃣ Mapeas la lista paginada a DTOs
        var dtoList = productos
            .Select(p => new ProductoSummaryDto
            (
                p.Id,
                p.Nombre,
                p.Codigo,
                p.Descripcion,
                p.PrecioVenta,
                p.CostoUnitario,
                p.UnidadMedida,
                p.EsPerecedero,
                p.CategoriaNombre,
                p.ProveedorNombre,
                p.ImagenUrl,
                p.Tags,
                p.Activo
            ))
            .ToList();

        // 3️⃣ Construyes una nueva PagedList usando los datos originales
        var result = new PagedList<ProductoSummaryDto>(
            dtoList,
            productos.TotalCount,
            productos.CurrentPage,
            productos.PageSize
        );

        return Ok(result);
    }
}
