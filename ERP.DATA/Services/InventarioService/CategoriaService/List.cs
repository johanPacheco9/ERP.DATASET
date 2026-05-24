
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace ERP.DATA.Services.InventarioService.CategoriaService;

public partial class CategoriaService
{
    public async Task<PagedList<CategoriaDetailDto>> List(
        ListCategoriasRequest request,
        CancellationToken cancellationToken)
    {
		try
		{

            var query = context.Category
           .AsNoTracking()
           .AsQueryable();

            var orderBy = request.OrderBy?.ToLower() ?? "id";

            query = orderBy switch
            {
                var o when o.Contains("nombre") && o.Contains("desc")
                    => query.OrderByDescending(p => p.Name),

                var o when o.Contains("nombre")
                    => query.OrderBy(p => p.Name),

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
                    p.Name,
                    p.Description,
                    p.CreatedAt,
                    p.UpdatedAt
                ));
            
            if (request.PageSize == -1)
            {
                var items = await dtoQuery.ToListAsync(cancellationToken);
                return new PagedList<CategoriaDetailDto>(items, items.Count, 1, items.Count);
            }
            return await PagedList<CategoriaDetailDto>.ToPagedListAsync(
                dtoQuery,
                request.PageNumber,
                request.PageSize
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en CategoriaService.List");
            throw; // sin return null, que reviente y nos diga qué es
        }
    }
}
