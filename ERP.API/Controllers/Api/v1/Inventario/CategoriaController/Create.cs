using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers.Api.v1.Inventario.CategoriaController;

public sealed class CreateCategoriaEndpoint : BaseCreateEndpoint<CreateCategoriaRequest, CreateCategoriaEndpoint>
{
    private readonly ICategoriaService _categoriaService;
    public CreateCategoriaEndpoint(ILogger<CreateCategoriaEndpoint> logger, ICategoriaService categoriaService) : base(logger)
    {
        _categoriaService = categoriaService;
    }

    [Tags("Inventario - Categorias")]
    [HttpPost(CategoriasEndpoints.List)]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateCategoriaRequest form,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(form, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateCategoriaRequest form, CancellationToken cancellationToken)
    {
        if (!form.ParametersAreValid(out var validationErrors))
            return BadRequest(new { errors = validationErrors });

        // Solo mapear del request a la entidad mínima
        var categoria = new Category
        {
            Name = form.Nombre,
            Description = form.Descripcion,
            Code = form.codigo // opcional, el servicio decide si genera uno
        };

        // Llamar al servicio
        var categoriaCreada = await _categoriaService.AddCategoriasAsync(categoria);

        return CreatedAtRoute("GetCategoriaById", new { id = categoriaCreada.Id }, new
        {
            categoriaCreada.Id,
            categoriaCreada.Code,
            categoriaCreada.Name,
            message = "Categoría creada exitosamente"
        });
    }

}