using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
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
        throw new NotImplementedException();
    }
}
