using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
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
        [FromBody] CreateCategoriaRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateCategoriaRequest request, CancellationToken cancellationToken)
    {
        // 1. Validar el request
        if (!request.ParametersAreValid(out var validationErrors))
        {
            return BadRequest(new { errors = validationErrors });
        }
        var codigo = string.IsNullOrWhiteSpace(request.codigo)
            ? $"CAT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}"
            : $"CAT-{request.codigo[..3].ToUpper()}";
        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Codigo = codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "01", // Esto debería venir del usuario autenticado
            UpdatedBy = null,
            UpdatedAt = null
        };

        // 5. Llamar al servicio para crear la categoría
        var categoriaCreada = await _categoriaService.AddCategoriasAsync(categoria);

        return CreatedAtRoute("GetCategoriaById", new { id = categoriaCreada.Id }, new
        {
            id = categoriaCreada.Id,
            codigo = categoriaCreada.Codigo,
            nombre = categoriaCreada.Nombre,
            message = "Categoría creada exitosamente"
        });
    }
}