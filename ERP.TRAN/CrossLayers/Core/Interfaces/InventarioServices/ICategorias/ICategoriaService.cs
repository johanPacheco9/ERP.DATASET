
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;

public interface ICategoriaService
{
    Task<Categoria> AddCategoriasAsync(Categoria categorias);
    Task<Categoria> GetCategoriaByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Categoria>> GetAllCategoriasAsync(CancellationToken cancellationToken);
    Task<Categoria> UpdateCategoriaAsync(Categoria categoria, CancellationToken cancellationToken);
    Task<bool> DeleteCategoriaAsync(Guid id, CancellationToken cancellationToken);
}


