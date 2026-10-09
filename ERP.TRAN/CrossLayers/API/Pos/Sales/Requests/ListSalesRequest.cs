using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Literals;

namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;

public sealed class ListSalesRequest : BaseListRequest
{
    /// <summary>Texto a buscar en número de venta, cliente, identificación o factura.</summary>
    public string? Search { get; set; }

    /// <summary>Filtra por bodega (opcional).</summary>
    public int? WarehouseId { get; set; }

    /// <summary>Fecha mínima de creación (opcional).</summary>
    public DateTime? MinDate { get; set; }

    /// <summary>Fecha máxima de creación (opcional).</summary>
    public DateTime? MaxDate { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();

        if (PageNumber < 1)
            list.Add("El número de página debe ser mayor o igual a 1.");

        if (PageSize == 0 || PageSize < PaginationLiterals.UnlimitedResultsPageSizeFlag)
            list.Add("El tamaño de página no es válido. Usa -1 si deseas obtener todos los registros.");

        if (MinDate.HasValue && MaxDate.HasValue && MinDate > MaxDate)
            list.Add("La fecha mínima no puede ser mayor que la fecha máxima.");

        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}