using ERP.API.Controllers.Utilities.Base;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers.Api.v1.Inventario.CategoriaController;

public sealed class CreateCategoriaEndpoint(IServiceProvider serviceProvider)
    : BaseCreateEndpoint<CreateCategoriaRequest, CreateCategoriaEndpoint>(serviceProvider)
{
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
        var codigo = $"CAT-{request.codigo[..3].ToUpper()}";

        var exists = await Repository.Categorias.AnyAsync(c => c.Codigo == codigo, cancellationToken);
        if (exists)
            return Conflict($"Ya existe una categoria con el código '{codigo}'.");

        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Codigo = codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "01",
            UpdatedBy = null,

        };

        Repository.Categorias.Add(categoria);

        await Repository.SaveChangesAsync(cancellationToken);

        return Created();
    }
}

