using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

/// <summary>
/// Tabla intermedia para la relación muchos a muchos entre ProductoBase y Category.
/// </summary>
public class ProductoBaseCategory
{
    public int Id { get; set; }
    public int ProductoBaseId { get; set; }
    public ProductoBase ProductoBase { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}