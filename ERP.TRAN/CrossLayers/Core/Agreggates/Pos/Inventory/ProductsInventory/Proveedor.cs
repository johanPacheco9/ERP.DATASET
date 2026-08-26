using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

public class Proveedor : EntityWithtraceability
{
    public string Name { get; set; } = null!;
    public string? Nit { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public ICollection<ProductoBase> Products { get; set; } = new List<ProductoBase>();
}
