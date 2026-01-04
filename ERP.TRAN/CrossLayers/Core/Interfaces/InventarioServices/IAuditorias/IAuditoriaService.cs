using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditoriasInventary;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAuditorias;

public interface IAuditoriaService
{
    Task<AuditoriaProductos> CreateAudit(AuditoriaProductos auditoriaProductos);

    Task<bool> UpdateAuditoria(AuditoriaProductos auditoriaProductos);

    Task<List<AuditoriaProductos>> ListAuditorias();
}
