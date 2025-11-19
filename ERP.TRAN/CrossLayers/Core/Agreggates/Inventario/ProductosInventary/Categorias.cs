using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
public class Categoria : EntityWithtraceability
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string Codigo { get; set; } = null!;
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
