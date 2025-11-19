using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Structs;


/// <summary>
///     Encabezados de paginación.
/// </summary>
public struct PaginationHeaders
{
    /// <summary>
    ///     Total de elementos.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    ///     Tamaño de página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    ///     Página actual (comienza en 1).
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    ///     Total de páginas
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    ///     Hay una página siguiente.
    /// </summary>
    public bool HasNext { get; set; }

    /// <summary>
    ///     Hay una página anterior.
    /// </summary>
    public bool HasPrevious { get; set; }
}

