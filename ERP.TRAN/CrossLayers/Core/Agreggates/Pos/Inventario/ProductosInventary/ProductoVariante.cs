using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
using System.ComponentModel;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;

public class ProductoVariante : EntityWithtraceability
{
    //Relación base
    public int ProductoId { get; set; }
    //Identificación
    [DisplayName("SKU")]
    public string CodigoVariante { get; set; } = null!; // SKU único
    public string? Codigo_Barras { get; set; }
    public string? Lote { get; set; }

    //Control de caducidad (solo si aplica)
    public DateTime? Fecha_Vencimiento { get; set; }

    //Costos y precios específicos de la variante
    public decimal? Precio_Venta { get; set; }
    public decimal? Costo_Unitario { get; set; }

    //Atributos dinámicos (color, talla, sabor, material, etc.)
    public string? Atributos { get; set; } // JSON o string tipo "Color=Rojo;Talla=M"

    //Relaciones
    public Producto Producto { get; set; } = null!;
    public ICollection<StockBodega> StockEnBodegas { get; set; } = new List<StockBodega>();
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>(); //relación directa con movimientos
}
