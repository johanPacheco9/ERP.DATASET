using System.ComponentModel.DataAnnotations.Schema;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

public class MovementConsumption : EntityWithtraceability
{
    public int ExitMovementId { get; set; }
    public Movement ExitMovement { get; set; } = null!;

    public int EntryMovementId { get; set; }
    public Movement EntryMovement { get; set; } = null!;

    public int QuantityConsumed { get; set; }

    [Column(TypeName = "decimal(15,4)")]
    public decimal UnitCost { get; set; }
}
