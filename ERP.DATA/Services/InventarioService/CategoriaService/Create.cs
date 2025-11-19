using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.CategoriaService;

public partial class CategoriaService
{
    public async Task<Categoria> AddCategoriasAsync(Categoria categorias)
    {
        try
        {
            // Validar el código recibido
            var codigo = string.IsNullOrWhiteSpace(categorias.Codigo)
                ? $"CAT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}" // genera uno nuevo
                : $"CAT-{categorias.Codigo[..Math.Min(3, categorias.Codigo.Length)].ToUpper()}"; // evita errores si tiene menos de 3 chars

            var categoria = new Categoria
            {
                Id = Guid.NewGuid(),
                Codigo = codigo,
                Nombre = categorias.Nombre,
                Descripcion = categorias.Descripcion,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = "01", // TODO: usuario autenticado
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
