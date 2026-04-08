namespace ERP.API.Controllers.Api.v1.Inventario.AuditController;

using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Audit;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAudit;
using Microsoft.AspNetCore.Mvc;

public sealed class GetAuditByIdEndpoint : BaseGetEndpoint<GetAuditByIdRequest, GetAuditByIdEndpoint, AuditDetailDto>
{
    private readonly IAuditService _auditService;

    public GetAuditByIdEndpoint(ILogger<GetAuditByIdEndpoint> logger, IAuditService auditService)
        : base(logger)
    {
        _auditService = auditService;
    }

    [Tags("Inventory - Audits")]
    [HttpGet(AuditEndpoints.Get, Name = "GetAuditById")]
    public override async Task<ActionResult<AuditDetailDto>> HandleAsync(
        [FromRoute] GetAuditByIdRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult<AuditDetailDto>> GetEntity(
        GetAuditByIdRequest request,
        CancellationToken cancellationToken)
    {
        var audit = await _auditService.GetAuditById(request.Id, cancellationToken);

        if (audit == null)
            return EntityNotFound("Audit", request.Id);

        TraceFound("Audit", audit.Id);
        return Ok(audit);
    }
}
