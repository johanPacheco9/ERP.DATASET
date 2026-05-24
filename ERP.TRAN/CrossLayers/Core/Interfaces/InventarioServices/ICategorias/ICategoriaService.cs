
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;

public interface ICategoriaService
{
    Task<Category> AddCategoriasAsync(Category categorias);
    Task<CategoriaDetailDto> GetById(GetCategoriaByIdRequest  request, CancellationToken cancellationToken);
    Task<PagedList<CategoriaDetailDto>> ListAsync(ListCategoriasRequest request, CancellationToken cancellationToken);
    Task<Category> UpdateCategoriaAsync(Category categoria, CancellationToken cancellationToken);
    Task<bool> DeleteCategoriaAsync(int id, CancellationToken cancellationToken);
}


