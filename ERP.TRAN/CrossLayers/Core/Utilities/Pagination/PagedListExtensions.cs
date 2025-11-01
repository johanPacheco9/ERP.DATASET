using ERP.TRAN.CrossLayers.Core.Utilities.Literals;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace ERP.TRAN.CrossLayers.Core.Utilities.Pagination;

public static class PagedListExtensions
{
    public static async Task<PagedList<TD>> PaginateAsync<TA, TD>(
        this IQueryable<TA> source,
        Expression<Func<TA, TD>> converter,
        int pageNumber,
        int pageSize)
    {
        var count = await source.CountAsync();

        List<TD> items;

        if (pageSize == PaginationLiterals.UnlimitedResultsPageSizeFlag)
        {
            items = await source.Skip(pageNumber - 1)
                .Select(converter)
                .ToListAsync();
        }
        else if (pageSize > 0)
        {
            items = await source.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(converter)
                .ToListAsync();
        }
        else
        {
            throw new Exception("El tamaño de página debe ser mayor a 0.");
        }

        return new PagedList<TD>(items, count, pageNumber, pageSize);
    }
}
