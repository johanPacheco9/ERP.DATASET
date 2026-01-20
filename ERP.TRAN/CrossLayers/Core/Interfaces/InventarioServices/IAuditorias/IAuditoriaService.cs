using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditoriasInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAuditorias;

public interface IAuditoriaService
{
    Task<UnitProductAudit> CreateAudit(UnitProductAudit auditoriaProductos);

    Task<bool> UpdateAuditoria(UnitProductAudit auditoriaProductos);

    Task<List<UnitProductAudit>> ListAuditorias();
}
