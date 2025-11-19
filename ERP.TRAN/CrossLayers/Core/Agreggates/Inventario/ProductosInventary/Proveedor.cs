using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
public class Proveedor : EntityWithtraceability
{
    public string Nombre { get; set; } = null!;
    public string? Nit { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
