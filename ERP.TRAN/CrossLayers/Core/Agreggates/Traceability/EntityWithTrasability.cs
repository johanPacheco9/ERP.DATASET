namespace ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
public class EntityWithtraceability
{

    public int Id { get; set; }
    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}

