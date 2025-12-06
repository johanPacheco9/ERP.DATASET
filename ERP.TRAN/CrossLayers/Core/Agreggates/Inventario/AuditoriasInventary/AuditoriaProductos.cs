using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.AuditoriasInventary;

public class AuditoriaProductos : EntityWithtraceability
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Detalles { get; set; } = null!;

    public AuditoriaStatus AuditoriaStatus { get; set; }
}

