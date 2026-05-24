using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;

public class UnitProductAudit : EntityWithtraceability
{
    public int AuditId { get; set; }
    public int UnitProductId { get; set; }
    public int LineaProductoId { get; set; }
    public int ProductoId { get; set; }
    public int BodegaId { get; set; }

    public string Serial { get; set; } = null!;

    public UnitProductAuditStatus Status { get; set; }

    public string? Observaciones { get; set; }
    public string? MotivoDiferencia { get; set; }

    public int? BodegaEncontrada { get; set; }
    public string? UbicacionFisica { get; set; }

    public string? EstadoFisico { get; set; } 
    public bool RequiereAccionCorrectiva { get; set; }
    public bool AjusteRealizado { get; set; }
    public int? MovimientoAjusteId { get; set; }
    public DateTime? FechaAjuste { get; set; }

    public Audit Audit { get; set; } = null!;
    public LineaProducto LineaProducto { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
    public Warehouse Bodega { get; set; } = null!;
    public Warehouse? BodegaEncontradaNav { get; set; }
}

