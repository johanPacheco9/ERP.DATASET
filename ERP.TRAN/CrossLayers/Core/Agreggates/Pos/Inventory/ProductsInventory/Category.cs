using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
public class Category : EntityWithtraceability
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Code { get; set; } = null!;
    public ICollection<LineaProducto> Products { get; set; } = new List<LineaProducto>();
}
