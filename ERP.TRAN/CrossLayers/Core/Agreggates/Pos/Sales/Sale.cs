using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Payments;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

public class Sale : EntityWithtraceability
{
    public string SaleNumber { get; set; } = null!;
    public int ClientId { get; set; }
    public int WarehouseId { get; set; }
    public int StoreId { get; set; }
    public int? PosTerminalId { get; set; }
    public int? PosShiftId { get; set; }
    public SaleStatus Status { get; set; }
    
    // Totales e Impuestos (Factus / DIAN)
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    public Client Client { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public Store Store { get; set; } = null!;
    public PosTerminal? PosTerminal { get; set; }
    public PosShift? PosShift { get; set; }
    
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    
    // Integración Facturación Electrónica (Factus / DIAN)
    public string? FactusInvoiceNumber { get; set; }
    public string? FactusStatus { get; set; }
    public string? FactusCufe { get; set; }
    public string? FactusQrUrl { get; set; }
    public string? FactusXmlUrl { get; set; }
    public string? FactusPdfUrl { get; set; }
    public string? FactusErrorMessage { get; set; }
    
    public ICollection<SalePayment> Payments { get; set; } = new List<SalePayment>();
    public ICollection<SaleLineItem> Lines { get; set; } = new List<SaleLineItem>();
}
