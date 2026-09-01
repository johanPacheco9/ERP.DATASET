using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;

public class AuditCategory : EntityWithtraceability
{
    public int AuditId { get; set; }
    public Audit Audit { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}