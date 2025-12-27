using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;

public class Movimiento : EntityWithtraceability
{
    public int? ProductoVarianteId { get; set; }

    public int? ProductoId { get; set; }
    public int BodegaId { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }

    public int Cantidad { get; set; }

    // Para FIFO/LIFO
    public decimal CostoUnitario { get; set; } // DECIMAL(15,4)

    // Calculado en memoria
    public decimal CostoTotal => Cantidad * CostoUnitario;

    // Referencias a otros módulos (compra, venta, etc.)
    public int? ReferenciaId { get; set; }
    public string? ReferenciaTipo { get; set; } // 'compra','venta','ajuste_manual', etc.

    // Lote / vencimiento
    public string? Lote { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    // Motivo y observaciones
    public string? Motivo { get; set; }
    public string? Observaciones { get; set; }

    //
    public Producto Producto { get; set; } = null!;
    public ProductoVariante ProductoVariante { get; set; } = null!;
    public Bodega Bodega { get; set; } = null!;
}
