namespace ERP.TRAN.CrossLayers.Core.Interfaces.Pagination;

/// <summary>
///     Contrato de una respuesta de listado de items con paginación.
/// </summary>
/// <typeparam name="T">Tipo del listado</typeparam>
public interface IPaginatedListResponse<T>
{
    /// <summary>
    ///     Página actual (comienza en 1).
    /// </summary>
    int CurrentPage { get; }

    /// <summary>
    ///     Total de páginas.
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    ///     Tamaño de página especificado.
    /// </summary>
    /// <remarks>
    ///     Aunque el usuario lo intente, no puede sobreescribir el máximo.
    /// </remarks>
    int PageSize { get; }

    /// <summary>
    ///     Total de items filtrados.
    /// </summary>
    int TotalFilteredCount { get; }

    /// <summary>
    ///     Hay una página anterior.
    /// </summary>
    bool HasPrevious { get; }

    /// <summary>
    ///     Hay una página siguiente.
    /// </summary>
    bool HasNext { get; }

    /// <summary>
    ///     Listado de items.
    /// </summary>
    List<T> Items { get; }
}