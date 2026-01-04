using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.CategoriaService;
//Cambiar a request, solo trabajar con dtos.
public partial class CategoriaService
{
    public async Task<Categoria> AddCategoriasAsync(Categoria categorias)
    {
        try
        {
            var codigo = string.IsNullOrWhiteSpace(categorias.Codigo)
                ? $"CAT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}"
                : $"CAT-{categorias.Codigo[..Math.Min(3, categorias.Codigo.Length)].ToUpper()}";

            var categoria = new Categoria
            {
                Codigo = codigo,
                Nombre = categorias.Nombre,
                Descripcion = categorias.Descripcion,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "01",
                UpdatedBy = null,
                UpdatedAt = null
            };

            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();

            return categoria;
        }
        catch (Exception ex)
        {
            _logger.LogTrace(ex.Message);
            return null;
        }
    }
}
