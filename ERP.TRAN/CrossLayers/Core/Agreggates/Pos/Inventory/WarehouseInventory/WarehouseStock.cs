using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

public class WarehouseStock : EntityWithtraceability
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    
    // Clave Foránea directa a la Variante (SKU)
    public int ProductoVarianteId { get; set; }

    // Totales calculados (Saldos agregados)
    public int CurrentStock { get; set; }    // Cantidad física disponible en la bodega
    public int StockReservado { get; set; } // Cantidad apartada/en transito/en carrito
    
    // Configuración de alertas por bodega
    public int StockMinimo { get; set; } = 0;
    public int StockMaximo { get; set; } = 0;

    public DateTime FechaActualizacion { get; set; }

    // Navegación
    public Warehouse Warehouse { get; set; } = null!;
    public ProductoVariante ProductoVariante { get; set; } = null!;
}