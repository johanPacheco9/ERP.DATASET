using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;

/// <summary>
/// Representa el registro de auditoría para una unidad de producto física o variante dentro de un proceso de inventario.
/// </summary>
public class UnidadProductoAuditada : EntityWithtraceability
{
    public int AuditId { get; set; }
    public int UnitProductId { get; set; }
    
    // Mapeo adaptado a la nueva arquitectura (ProductoBase y ProductoVariante)
    public int ProductoBaseId { get; set; }
    public int ProductoVarianteId { get; set; }
    public int BodegaId { get; set; }

    public string Serial { get; set; } = null!;

    public UnitProductAuditStatus Status { get; set; } // Estado dentro de la auditoría (Found, NotFound, etc.)
    
    // NUEVO: Guarda el estado físico que tenía la unidad antes de ser bloqueada
    public UnidadProductoStatus OriginalUnitStatus { get; set; }

    public string? Observaciones { get; set; }
    public string? MotivoDiferencia { get; set; }

    public int? BodegaEncontrada { get; set; }
    public string? UbicacionFisica { get; set; }

    public string? EstadoFisico { get; set; }  
    public bool RequiereAccionCorrectiva { get; set; }
    public bool AjusteRealizado { get; set; }
    public int? MovimientoAjusteId { get; set; }
    public DateTime? FechaAjuste { get; set; }

    // === NAVEGACIÓN ===
    public Audit Audit { get; set; } = null!;
    public ProductoBase ProductoBase { get; set; } = null!;
    public ProductoVariante ProductoVariante { get; set; } = null!;
    public Warehouse Bodega { get; set; } = null!;
    public Warehouse? BodegaEncontradaNav { get; set; }
}