
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace ERP.DATA.Services.InventarioService.CategoriaService;

public partial class CategoriaService
{
    public async Task<PagedList<CategoriaDetailDto>> ListAsync(
        ListCategoriasRequest request,
        CancellationToken cancellationToken)
    {
		try
		{

            var query = _context.Categorias
           .AsNoTracking()
           .AsQueryable();

            query = request.OrderBy?.ToLower() switch
            {
                var o when o.Contains("nombre") && o.Contains("desc")
                    => query.OrderByDescending(p => p.Nombre),

                var o when o.Contains("nombre")
                    => query.OrderBy(p => p.Nombre),

                var o when o.Contains("id") && o.Contains("desc")
                    => query.OrderByDescending(p => p.Id),

                _ => query.OrderBy(p => p.Id)
            };


            if (request.MinDate is not null)
                query = query.Where(p => p.CreatedAt >= request.MinDate.Value);

            if (request.MaxDate is not null)
                query = query.Where(p => p.CreatedAt <= request.MaxDate.Value);

            var dtoQuery = query
                .Select(p => new CategoriaDetailDto
                (
                    p.Id,
                    p.Nombre,
                    p.Descripcion,
                    p.CreatedAt,
                    p.UpdatedAt
                ));

            return await PagedList<CategoriaDetailDto>.ToPagedListAsync(
                dtoQuery,
                request.PageNumber,
                request.PageSize
            );
        }
		catch (Exception ex)
		{
           _logger.LogError(ex.Message);
            return null;
		}
    }
}
