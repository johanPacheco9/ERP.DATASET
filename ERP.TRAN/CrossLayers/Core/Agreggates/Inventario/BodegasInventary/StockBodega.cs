using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;

public class StockBodega : EntityWithtraceability
{
    public Guid? ProductoVarianteId { get; set; }
    public Guid BodegaId { get; set; }

    public Guid? ProductoId { get; set; }

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