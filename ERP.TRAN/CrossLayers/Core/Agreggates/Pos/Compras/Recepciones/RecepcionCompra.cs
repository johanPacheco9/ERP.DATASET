using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras.Recepciones;

public enum RecepcionCompraStatus
{
    [Display(Name = "Borrador")]
    Borrador = 0,
    [Display(Name = "Fisico")]
    RecibidoFisico = 1,
    [Display(Name = "En control de calidad")]
    EnControlCalidad = 2,
    
    [Display(Name = "Finalizado")]
    Finalizado = 3,
    [Display(Name = "Rechazado parcialmente")]
    RechazadoParcial = 4
}

public class RecepcionCompra : EntityWithtraceability
{
    public int Id { get; set; }

    public int OrdenCompraId { get; set; }
    public OrdenCompra OrdenCompra { get; set; } = null!;

    public int BodegaId { get; set; } // ¿A qué almacén/bodega ingresa físicamente?

    public DateTime FechaRecepcion { get; set; } = DateTime.UtcNow;

    public RecepcionCompraStatus Status { get; set; } = RecepcionCompraStatus.RecibidoFisico;

    public string? GuiaRemisionProveedor { get; set; } // Número de albarán o remisión del proveedor
    public string? Observaciones { get; set; }

    public ICollection<DetalleRecepcionCompra> Detalles { get; set; } = new List<DetalleRecepcionCompra>();
}
