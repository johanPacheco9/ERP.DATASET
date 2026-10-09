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

    public string? Serial { get; set; }

    public int? UnidadProductoId { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal? CashReceived { get; set; }

    public List<SaleLineRequest> Lines { get; set; } = new();

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (ClientId <= 0)
            list.Add("Seleccione un cliente.");
        if (WarehouseId <= 0)
            list.Add("Seleccione una bodega.");
        // NUEVO (#16): StoreId tiene default 1, pero si llega 0 o negativo no debe pasar.
        if (StoreId <= 0)
            list.Add("Seleccione una tienda.");
        // FIX: PaymentAmount no se validaba y podía llegar negativo, contaminando
        // los cálculos de saldo/estado de pago desde la creación de la venta.
        if (PaymentAmount < 0)
            list.Add("El monto de pago no puede ser negativo.");
        // NUEVO (#16): el efectivo recibido no puede ser negativo ni menor al pago en efectivo.
        if (CashReceived.HasValue)
        {
            if (CashReceived.Value < 0)
                list.Add("El efectivo recibido no puede ser negativo.");
            else if (PaymentMethod == PaymentMethod.Cash && CashReceived.Value < PaymentAmount)
                list.Add("El efectivo recibido no puede ser menor al monto del pago.");
        }
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
                // NUEVO (#16): un precio negativo restaría del total de la venta.
                if (line.UnitPrice.HasValue && line.UnitPrice.Value < 0)
                    list.Add($"Línea {i}: el precio unitario no puede ser negativo.");
                // NUEVO (#16): la tarifa es una fracción (0.19 = 19%). Si llega 19 en vez de
                // 0.19, el IVA saldría 19 veces el valor de la línea.
                if (line.TaxRate.HasValue && (line.TaxRate.Value < 0 || line.TaxRate.Value > 1))
                    list.Add($"Línea {i}: la tarifa de IVA debe estar entre 0 y 1 (por ejemplo 0.19 para 19%).");
            }
        }

        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}

public sealed class SaleLineRequest
{
    public int ProductoVarianteId { get; set; }
    public string? SerialNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? UnitPrice { get; set; }
    public decimal? TaxRate { get; set; }
}