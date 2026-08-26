using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.API.Controllers.Api.v1.Inventario.ProductoController;

/// <summary>
/// Endpoint para listar productos con filtros y paginación.
/// </summary>
public sealed class List
    : BaseListEndpoint<ListProductRequest, List, PagedList<ProductoSummaryDto>>
{
    private readonly ProductoBaseService _productoService;

    public List(
        ILogger<List> logger,
        ProductoBaseService productoService
    ) : base(logger)
    {
        _productoService = productoService;
    }

    [Tags("Inventario - Productos")]
    [HttpGet(ProductEndpoints.List, Name = "List Productos")]
    public override async Task<ActionResult<PagedList<ProductoSummaryDto>>> HandleAsync(
        [FromQuery] ListProductRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    /// <summary>
    /// Lógica para listar productos desde la base de datos.
    /// </summary>
    protected override async Task<ActionResult<PagedList<ProductoSummaryDto>>> ListEntity(
        ListProductRequest request,
        CancellationToken cancellationToken
    )
    {
        var productosPaged = await _productoService.ListAsync(request, cancellationToken);

        var dtoItems = productosPaged
            .Select(p => new ProductoSummaryDto(
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
                p.Activo,
                p.ProductoVariantes.Select(v => new ProductoVarianteDetailDto(
                    v.Id,
                    v.CodigoVariante,
                    v.Atributos,
                    v.PrecioVenta,
                    v.CostoUnitario,
                    v.Stock,
                    v.StockMinimo,
                    v.CodigoBarras,
                    v.Activo
                )).ToList()
            ))
            .ToList();

        var result = new PagedList<ProductoSummaryDto>(
            dtoItems,
            productosPaged.TotalCount,
            productosPaged.CurrentPage,
            productosPaged.PageSize
        );

        return Ok(result);
    }
}
