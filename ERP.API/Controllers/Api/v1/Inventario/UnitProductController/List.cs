using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Producto;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IUnitProduct;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.UnitProductController;

public sealed class List
    : BaseListEndpoint<ListUnitProductRequest, List, PagedList<UnitProductDetailDto>>
{
    private readonly IUnitProductService _unitProductService;

    public List(
        ILogger<List> logger,
        IUnitProductService unitProductService
    ) : base(logger)
    {
        _unitProductService = unitProductService;
    }

    [Tags("Inventario -UnitProduct")]
    [HttpGet(UnitProductEndpoints.List, Name = "List UnitProducts")]
    public override async Task<ActionResult<PagedList<UnitProductDetailDto>>> HandleAsync(
        [FromQuery] ListUnitProductRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    /// <summary>
    /// Lógica para listar unidades de producto desde la base de datos.
    /// </summary>
    protected override async Task<ActionResult<PagedList<UnitProductDetailDto>>> ListEntity(
        ListUnitProductRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await _unitProductService.ListAsync(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener las unidades de producto",
                error = ex.Message
            });
        }
    }
}