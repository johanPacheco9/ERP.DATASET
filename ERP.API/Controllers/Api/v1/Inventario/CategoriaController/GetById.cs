using ERP.TRAN.CrossLayers.API.Inventario.Categoria;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.CategoriaController;

public sealed class GetCategoriaByIdEndpoint
    : BaseGetEndpoint<GetCategoriaByIdRequest, GetCategoriaByIdEndpoint, CategoriaDetailDto>
{
    private readonly ICategoriaService _categoriaService;


    public GetCategoriaByIdEndpoint(ICategoriaService categoriaService, ILogger<GetCategoriaByIdEndpoint> logger) : base(logger)
    {
        _categoriaService = categoriaService;
    }
   
    [Tags("Inventario - Categorias")]
    [HttpGet(CategoriasEndpoints.Get, Name = "Get categoria by id")]
    public override async Task<ActionResult<CategoriaDetailDto>> HandleAsync(
        [FromQuery] GetCategoriaByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }
    protected override async Task<ActionResult<CategoriaDetailDto>> GetEntity(GetCategoriaByIdRequest request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaService.GetCategoriaByIdAsync(request.Id, cancellationToken);
        if (categoria == null)
        {
            return NotFound($"Categoría con ID {request.Id} no encontrada");
        }
        var categoriaDto = new CategoriaDetailDto
        (
            categoria.Id,
            categoria.Nombre,
            categoria.Descripcion,
            categoria.CreatedAt,
            categoria.UpdatedAt
        );
        TraceFound(nameof(Categoria), request.Id);
        return Ok(categoriaDto);
    }
}
