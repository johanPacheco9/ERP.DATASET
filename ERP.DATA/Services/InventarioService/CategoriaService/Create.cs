using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.CategoriaService;
/// <summary>
/// Si crece, manejar dtos.
/// </summary>
public partial class CategoriaService
{
    public async Task<Category?> AddCategoriasAsync(CreateCategoriaRequest request)
    {
        if (!request.ParametersAreValid(out var validationError))
        {
            logger.LogWarning("Parámetros inválidos para categoría: {Error}", validationError);
            return null;
        }

        try
        {
            var codigoFinal = string.IsNullOrWhiteSpace(request.codigo)
                ? $"CAT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}"
                : $"CAT-{request.codigo[..Math.Min(3, request.codigo.Length)].ToUpper()}";

            var categoria = new Category
            {
                Code = codigoFinal,
                Name = request.Nombre,             // Usamos 'Nombre' del DTO
                Description = request.Descripcion, // Usamos 'Descripcion' del DTO
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "system",
                UpdatedAt = null,
                UpdatedBy = null
            };

            context.Category.Add(categoria);
            await context.SaveChangesAsync();

            return categoria;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al insertar categoría en la base de datos");
            return null;
        }
    }
}