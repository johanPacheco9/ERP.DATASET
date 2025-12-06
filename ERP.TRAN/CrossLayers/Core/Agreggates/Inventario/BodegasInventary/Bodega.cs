using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
public class Bodega : EntityWithtraceability
{
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Ubicacion { get; set; }
    public string? Descripcion { get; set; }
    public decimal? Capacidad_Maxima { get; set; }
    public TipoBodega TipoBodega { get; set; }

//Navegacion
    public ICollection<StockBodega> StockProductos { get; set; } = new List<StockBodega>();
    public ICollection<Movimiento> Movimiento { get; set; } = new List<Movimiento>();

}
