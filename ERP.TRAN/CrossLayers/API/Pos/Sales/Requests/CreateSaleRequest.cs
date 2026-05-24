using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;

public sealed class CreateSaleRequest : BaseCreateRequest
{
    public int ClientId { get; set; }
    public int WarehouseId { get; set; }
    public int StoreId { get; set; } = 1;
    public string? Notes { get; set; }
    public decimal PaymentAmount { get; set; }  
    
    public PaymentMethod PaymentMethod { get; set; }
    
    public List<SaleLineRequest> Lines { get; set; } = new();

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (ClientId <= 0)
            list.Add("Seleccione un cliente.");
        if (WarehouseId <= 0)
            list.Add("Seleccione una bodega.");
        if (Lines == null || !Lines.Any())
            list.Add("Agregue al menos una línea de venta.");
        else
        {
            foreach (var (line, i) in Lines.Select((l, idx) => (l, idx + 1)))
            {
                if (line.LineaProductoId <= 0)
                    list.Add($"Línea {i}: producto inválido.");
                if (line.Quantity <= 0)
                    list.Add($"Línea {i}: cantidad debe ser mayor a cero.");
            }
        }
        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}

public sealed class SaleLineRequest
{
    public int LineaProductoId { get; set; }
    public int? ProductoId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? UnitPrice { get; set; }
}
