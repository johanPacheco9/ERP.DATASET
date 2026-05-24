using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;


namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.Stores;

public class Store : EntityWithtraceability
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsMainStore{ get; set; }

    public ICollection<Warehouse> Bodegas { get; set; } = new List<Warehouse>();
}
