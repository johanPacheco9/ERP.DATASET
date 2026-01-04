
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;

public class UnitProduct : EntityWithtraceability
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public int ProductoVarianteId { get; set; }
    public int BodegaId { get; set; }
    public string Serial { get; set; } = null!;
    public UnitProductStatus UnitProductStatus { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaSalida { get; set; }

    // navegación
    public Producto Producto { get; set; } = null!;
    public ProductoVariante ProductoVariante { get; set; } = null!;
    public Bodega Bodega { get; set; } = null!;
    public ICollection<UnitProductMovement> Movimientos { get; set; } = new List<UnitProductMovement>();
}
