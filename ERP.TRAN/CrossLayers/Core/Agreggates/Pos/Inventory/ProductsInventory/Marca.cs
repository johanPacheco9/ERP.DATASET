using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;

/// <summary>
/// Representa la marca comercial asociada a uno o varios productos.
/// </summary>
public class Marca : EntityWithtraceability
{
    public int Id { get; set; }

    // === IDENTIDAD ===
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    // === INFORMACIÓN ADICIONAL ===
    public string? LogoUrl { get; set; }

    // === ESTADO ===
    public bool Activa { get; set; } = true;

    // === RELACIONES ===
    public ICollection<ProductoBase> Productos { get; set; }
        = new List<ProductoBase>();
}