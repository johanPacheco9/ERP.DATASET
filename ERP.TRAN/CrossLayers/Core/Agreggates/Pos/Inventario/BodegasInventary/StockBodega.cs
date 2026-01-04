using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;

public class StockBodega : EntityWithtraceability
{
    public int? ProductoVarianteId { get; set; }
    public int BodegaId { get; set; }

    public int? ProductoId { get; set; }

    public int StockActual { get; set; } = 0;
    public int StockMinimo { get; set; } = 0;
    public int StockMaximo { get; set; } = 0;
    public int StockReservado { get; set; } = 0;

    public DateTime FechaActualizacion { get; set; }

    // Navegación opcional
    public ProductoVariante? ProductoVariante { get; set; }
    public Producto? Producto { get; set; }
    public Bodega? Bodega { get; set; }
}