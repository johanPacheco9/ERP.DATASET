using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;

namespace ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAudit;

public interface IAuditService
{
    Task<AuditDetailDto> CreateAudit(CreateAuditRequest request, CancellationToken cancellationToken);

    Task<AuditDetailDto?> GetAuditById(int id, CancellationToken cancellationToken);

    Task<List<AuditSummaryDto>> ListAudits(CancellationToken cancellationToken = default);

    Task<bool> UpdateUnitProductAudit(UpdateUnitProductAuditRequest request, CancellationToken cancellationToken = default);
    Task<List<UnitProductAuditSummaryDto>> ListAuditorias(int? auditId = null, CancellationToken cancellationToken = default);
}
