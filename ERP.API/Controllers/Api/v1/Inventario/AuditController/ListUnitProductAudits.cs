namespace ERP.API.Controllers.Api.v1.Inventario.AuditController;

using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Audit;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAudit;
using Microsoft.AspNetCore.Mvc;

public sealed class ListUnitProductAuditsEndpoint
    : BaseListEndpoint<ListUnitProductAuditsRequest, ListUnitProductAuditsEndpoint, List<UnitProductAuditSummaryDto>>
{
    private readonly IAuditService _auditService;

    public ListUnitProductAuditsEndpoint(ILogger<ListUnitProductAuditsEndpoint> logger, IAuditService auditService)
        : base(logger)
    {
        _auditService = auditService;
    }

    [Tags("Inventory - Audits")]
    [HttpGet(AuditEndpoints.UnitProductAudits, Name = "ListUnitProductAudits")]
    public override async Task<ActionResult<List<UnitProductAuditSummaryDto>>> HandleAsync(
        [FromQuery] ListUnitProductAuditsRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<List<UnitProductAuditSummaryDto>>> ListEntity(
        ListUnitProductAuditsRequest request,
        CancellationToken cancellationToken)
    {
        var items = await _auditService.ListAuditorias(request.AuditId, cancellationToken);
        return Ok(items);
    }
}
