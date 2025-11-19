using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Literals;


/// <summary>
///     Literales para paginación generic.
/// </summary>
public class PaginationLiterals
{
    /// <summary>
    ///     Valor para indicar que no hay límite en la cantidad de resultados.
    /// </summary>
    /// <remarks>
    ///     Su uso debe estar restringido únicamente para la generación de archivos a descargar, nunca para la interface. 
    /// </remarks>
    public const int UnlimitedResultsPageSizeFlag = -1;
}
