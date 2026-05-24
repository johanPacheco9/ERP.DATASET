using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;

public class WarehouseStock : EntityWithtraceability
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public int LineaProductoId { get; set; } // Referencia al "Producto Base" o Catálogo

    // Totales calculados (Saldos)
    public int CurrentStock { get; set; }   // Suma de unidades con Status = Disponible
    public int StockReservado { get; set; } // Suma de unidades con Status = Reservado (en carrito/pedido)
    
    // Configuración de alertas
    public int StockMinimo { get; set; } = 0;
    public int StockMaximo { get; set; } = 0;

    public DateTime FechaActualizacion { get; set; }

    // Navegación
    public Warehouse Warehouse { get; set; } = null!;
    public LineaProducto LineaProducto { get; set; } = null!;
}