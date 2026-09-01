using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
public class Warehouse : EntityWithtraceability
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Ubication { get; set; }
    public string? Description { get; set; }
    public decimal? Max_Capacity { get; set; }
    public WarehouseType Type { get; set; }
    
    public WarehouseStatus Status { get; set; }

    public int StoreId { get; set; }
//Navegacion
    public ICollection<WarehouseStock> StockProductos { get; set; } = new List<WarehouseStock>();
    public ICollection<Movement> Movement { get; set; } = new List<Movement>();

    public Store Store { get; set; }

}
