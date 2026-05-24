using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
public class Supplier : EntityWithtraceability
{
    public string Name { get; set; } = null!;
    public string? Nit { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public ICollection<LineaProducto> Products { get; set; } = new List<LineaProducto>();
}
