namespace ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
public class EntityWithtraceability
{

    public int Id { get; set; }
    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}

