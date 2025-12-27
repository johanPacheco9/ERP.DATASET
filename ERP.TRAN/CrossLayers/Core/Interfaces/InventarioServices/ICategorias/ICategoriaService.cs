
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;

public interface ICategoriaService
{
    Task<Categoria> AddCategoriasAsync(Categoria categorias);
    Task<Categoria> GetCategoriaByIdAsync(int id, CancellationToken cancellationToken);
    Task<PagedList<CategoriaDetailDto>> ListAsync(ListCategoriasRequest request, CancellationToken cancellationToken);
    Task<Categoria> UpdateCategoriaAsync(Categoria categoria, CancellationToken cancellationToken);
    Task<bool> DeleteCategoriaAsync(int id, CancellationToken cancellationToken);
}


