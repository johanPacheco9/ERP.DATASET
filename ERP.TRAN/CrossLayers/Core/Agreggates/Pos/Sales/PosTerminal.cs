using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

/// <summary>
/// Representa una caja física o terminal de punto de venta (POS) dentro de una sucursal, 
/// encargada de gestionar el inventario de una bodega específica, controlar los consecutivos 
/// de facturación ante la DIAN y agrupar los turnos de operación.
/// </summary>
public class PosTerminal : EntityWithtraceability
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int StoreId { get; set; }
    public int WarehouseId { get; set; }
    public string Prefix { get; set; } = "POS1";
    public long CurrentConsecutive { get; set; } = 0;
    public string? DianResolutionNumber { get; set; }
    public string? DianResolutionDate { get; set; }
    public long FromNumber { get; set; } = 1;
    public long ToNumber { get; set; } = 999999;
    public new bool IsActive { get; set; } = true;

    public Store Store { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<PosShift> Shifts { get; set; } = new List<PosShift>();
}
