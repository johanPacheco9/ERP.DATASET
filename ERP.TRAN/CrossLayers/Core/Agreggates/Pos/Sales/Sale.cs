using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Payments;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

public class Sale : EntityWithtraceability
{
    public string SaleNumber { get; set; } = null!;
    public int ClientId { get; set; }
    public int WarehouseId { get; set; }
    public int StoreId { get; set; }
    public SaleStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    public Client Client { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public Store Store { get; set; } = null!;
    
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    
    public ICollection<SalePayment> Payments { get; set; } = new List<SalePayment>();
    public ICollection<SaleLineItem> Lines { get; set; } = new List<SaleLineItem>();
}
