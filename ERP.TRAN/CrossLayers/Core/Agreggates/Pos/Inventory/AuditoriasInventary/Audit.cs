using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;

public class Audit : EntityWithtraceability
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? WarehouseId { get; set; } 
    public int? ProductId { get; set; }

    public AuditType Type { get; set; }
    public AuditStatus Status { get; set; }

    public int ResponsibleId { get; set; }
    public int? SupervisorId { get; set; }

    public int TotalExpectedUnits { get; set; }
    public int TotalCountedUnits { get; set; }
    public int TotalMatches { get; set; }
    public int TotalMissing { get; set; }
    public int TotalSurplus { get; set; }
    public int TotalLocationDifferences { get; set; }
    public int TotalStatusDifferences { get; set; }

    // General observations
    public string? Observations { get; set; }
    public string? Conclusions { get; set; }

    public Warehouse? Warehouse { get; set; }
    public ICollection<AuditCategory> CategoriasAuditadas { get; set; } = new List<AuditCategory>();
    public ProductoBase? Product { get; set; }
    public ICollection<UnidadProductoAuditada> UnidadProductoAuditada {get; set; } = new List<UnidadProductoAuditada>();
    //Relacion para saber que movimientos generó la auditoria.
    public ICollection<Movement> Movements { get; set; } = new List<Movement>();
}
