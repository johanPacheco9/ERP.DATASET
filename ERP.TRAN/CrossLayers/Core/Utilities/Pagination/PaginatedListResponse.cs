using ERP.TRAN.CrossLayers.Core.Interfaces.Pagination;

namespace ERP.TRAN.CrossLayers.Utilities.Pagination;

/// <summary>
///     Respuesta de listado de items con paginación.
/// </summary>
/// <typeparam name="T">Tipo del listado</typeparam>
public sealed class PaginatedListResponse<T> : IPaginatedListResponse<T>
{
    /// <summary>
    ///     Crear una respuesta de listado de items con paginación.
    /// </summary>
    /// <param name="items">Items devueltos</param>
    /// <param name="totalFilteredCount">Total de ítems filtrados</param>
    /// <param name="currentPage">Página actual</param>
    /// <param name="pageSize">Tamaño de página</param>
    public PaginatedListResponse(List<T> items, int totalFilteredCount, int currentPage, int pageSize)
    {
        TotalFilteredCount = totalFilteredCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(TotalFilteredCount / (double)pageSize);

        // ReSharper disable  VirtualMemberCallInConstructor
        Items = new List<T>();
        Items.AddRange(items);
    }

    /// <summary>
    ///     Página actual.
    /// </summary>
    public int CurrentPage { get; private set; }

    /// <summary>
    ///     Total de páginas.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    ///     Tamaño de página especificado.
    /// </summary>
    /// <remarks>
    ///     Aunque el usuario lo intente, no puede sobreescribir el máximo.
    /// </remarks>
    public int PageSize { get; private set; }

    /// <summary>
    ///     Total de items filtrados.
    /// </summary>
    public int TotalFilteredCount { get; private set; }

    /// <summary>
    ///     Hay una página anterior.
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    ///     Hay una página siguiente.
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    /// <summary>
    ///     Listado de items.
    /// </summary>
    public List<T> Items { get; init; }
}