using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditoriasInventary;

public class UnitProductAudit : EntityWithtraceability
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Details { get; set; } = null!;

    public AuditoriaStatus AuditoriaStatus { get; set; }
}

