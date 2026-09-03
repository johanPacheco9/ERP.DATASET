using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;

public sealed class CreateSaleRequest : BaseCreateRequest
{
    public int ClientId { get; set; }
    public int WarehouseId { get; set; }
    public int StoreId { get; set; } = 1;
    public int? PosTerminalId { get; set; }
    public int? PosShiftId { get; set; }
    public string? Notes { get; set; }
    public decimal PaymentAmount { get; set; }  
    
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    
    public List<SaleLineRequest> Lines { get; set; } = new();

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (ClientId <= 0)
            list.Add("Seleccione un cliente.");
        if (WarehouseId <= 0)
            list.Add("Seleccione una bodega.");
            // FIX: PaymentAmount no se validaba y podía llegar negativo, contaminando
    // los cálculos de saldo/estado de pago desde la creación de la venta.
             if (PaymentAmount < 0)
        list.Add("El monto de pago no puede ser negativo.");
        if (Lines == null || !Lines.Any())
            list.Add("Agregue al menos un producto a la venta.");
        else
        {
            foreach (var (line, i) in Lines.Select((l, idx) => (l, idx + 1)))
            {
                if (line.ProductoVarianteId <= 0)
                    list.Add($"Línea {i}: variante de producto inválida.");
                if (line.Quantity <= 0)
                    list.Add($"Línea {i}: la cantidad debe ser mayor a cero.");
            }
        }
        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}

public sealed class SaleLineRequest
{
    public int ProductoVarianteId { get; set; }
    public string? SerialNumber { get; set; } // Añadido para dar soporte a productos serializados (UnidadProducto)
    public int Quantity { get; set; } = 1;
    public decimal? UnitPrice { get; set; }
    public decimal? TaxRate { get; set; }
}