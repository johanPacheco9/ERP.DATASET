using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;

/// <summary>
/// Clase para el registro de movimientos por unidad de producto
/// </summary>
public class UnitProductMovement
{
    public int Id { get; set; }
    
    public int BodegaOrigenId { get; set; }
    public int? BodegaDestinoId { get; set; }

    public string Motivo { get; set; } = null!;
    public string? Observaciones { get; set; }
    
    public TipoMovimiento TipoMovimiento { get; set; }
    
    /// <summary>
    /// Relaciones
    /// </summary>
    
    public int UnidadProductoId { get; set; }
    public UnidadProducto UnidadProducto { get; set; } = null!;
    
    public int MovimientoId { get; set; }
    public Movement Movimiento { get; set; } = null!;
}
