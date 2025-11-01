using ERP.API.Controllers.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers.Api.v1.Inventario.CategoriaController;

public sealed class GetCategoriaByIdEndpoint(IServiceProvider serviceProvider)
    : BaseGetEndpoint<GetCategoriaByIdRequest, GetCategoriaByIdEndpoint, CategoriaDetailDto>(serviceProvider)
{
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
        // Buscar la categoría en el repositorio por Id
        var categoria = await Repository.Categorias
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        // Si no existe, devolvemos 404 y registramos el evento
        if (categoria is null)
            return EntityNotFound(nameof(Categoria));

        // Mapear la entidad a DTO
        var categoriaDto = new CategoriaDetailDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            FechaCreacion = categoria.CreatedAt,
            FechaModificacion = categoria.UpdatedAt
        };

        // Registrar que se encontró correctamente
        TraceFound(nameof(Categoria), request.Id);

        // Devolver la categoría encontrada
        return Ok(categoriaDto);
    }
}
