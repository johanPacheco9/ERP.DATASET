using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

/// <summary>
/// Relación entre un producto y un proveedor.
/// Permite manejar múltiples proveedores y sus condiciones de compra.
/// </summary>
public class ProductoProveedor : EntityWithtraceability
{
    public int Id { get; set; }

    public int ProductoBaseId { get; set; }
    public int ProveedorId { get; set; }

    // === INFORMACIÓN COMERCIAL ===
    public decimal CostoUnitario { get; set; }

    public string? CodigoProveedor { get; set; }

    public int? DiasEntrega { get; set; }

    public bool EsPrincipal { get; set; }

    // === RELACIONES ===
    public ProductoBase ProductoBase { get; set; } = null!;
    public Proveedor Proveedor { get; set; } = null!;
}