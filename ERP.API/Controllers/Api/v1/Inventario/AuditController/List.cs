namespace ERP.API.Controllers.Api.v1.Inventario.AuditController;

using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Audit;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAudit;
using Microsoft.AspNetCore.Mvc;

public sealed class ListAuditsEndpoint
    : BaseListEndpoint<ListAuditsRequest, ListAuditsEndpoint, List<AuditSummaryDto>>
{
    private readonly IAuditService _auditService;

    public ListAuditsEndpoint(ILogger<ListAuditsEndpoint> logger, IAuditService auditService)
        : base(logger)
    {
        _auditService = auditService;
    }

    [Tags("Inventory - Audits")]
    [HttpGet(AuditEndpoints.List, Name = "ListAudits")]
    public override async Task<ActionResult<List<AuditSummaryDto>>> HandleAsync(
        [FromQuery] ListAuditsRequest request,
        CancellationToken cancellationToken = default)
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<List<AuditSummaryDto>>> ListEntity(
        ListAuditsRequest request,
        CancellationToken cancellationToken)
    {
        var audits = await _auditService.ListAudits(cancellationToken);
        return Ok(audits);
    }
}
