using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Literals;

namespace ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;

public sealed class ListClientsRequest : BaseListRequest
{
    /// <summary>Texto a buscar en nombre, identificación, correo o teléfono.</summary>
    public string? Search { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();

        if (PageNumber < 1)
            list.Add("El número de página debe ser mayor o igual a 1.");

        if (PageSize == 0 || PageSize < PaginationLiterals.UnlimitedResultsPageSizeFlag)
            list.Add("El tamaño de página no es válido. Usa -1 si deseas obtener todos los registros.");

        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}