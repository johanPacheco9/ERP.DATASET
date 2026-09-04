using ERP.TRAN.CrossLayers.Core.Utilities.Literals;
using ERP.TRAN.CrossLayers.Core.Utilities.Structs;
using Microsoft.EntityFrameworkCore;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Pagination;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize == 0 || pageSize < PaginationLiterals.UnlimitedResultsPageSizeFlag)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = pageSize == PaginationLiterals.UnlimitedResultsPageSizeFlag
            ? 1
            : (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = pageSize == PaginationLiterals.UnlimitedResultsPageSizeFlag
            ? source.ToList()
            : source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = pageSize == PaginationLiterals.UnlimitedResultsPageSizeFlag
            ? await source.ToListAsync()
            : await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public PaginationHeaders PaginationHeaders =>
        new()
        {
            TotalCount = TotalCount,
            PageSize = PageSize,
            CurrentPage = CurrentPage,
            TotalPages = TotalPages,
            HasNext = HasNext,
            HasPrevious = HasPrevious
        };
}
