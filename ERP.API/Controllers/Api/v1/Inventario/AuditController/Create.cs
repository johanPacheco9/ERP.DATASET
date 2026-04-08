namespace ERP.API.Controllers.Api.v1.Inventario.AuditController;

using ERP.DATA.Utilities.Providers;
using ERP.TRAN.CrossLayers.API.Inventario.Audit;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAudit;
using Microsoft.AspNetCore.Mvc;

public sealed class CreateAuditEndpoint : BaseCreateEndpoint<CreateAuditRequest, CreateAuditEndpoint>
{
    private readonly IAuditService _auditService;

    public CreateAuditEndpoint(ILogger<CreateAuditEndpoint> logger, IAuditService auditService)
        : base(logger)
    {
        _auditService = auditService;
    }

    [Tags("Inventory - Audits")]
    [HttpPost(AuditEndpoints.List, Name = "CrearAuditoria")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] CreateAuditRequest request,
        CancellationToken cancellationToken = new())
    {
        return await base.HandleAsync(request, cancellationToken);
    }

    protected override async Task<ActionResult> CreateEntity(CreateAuditRequest request, CancellationToken cancellationToken)
    {
        if (!request.ParametersAreValid(out var validationErrors))
            return BadRequest(new { errors = validationErrors });

        request._CreatorAuth0Id ??= User?.Identity?.Name ?? "system";

        var audit = await _auditService.CreateAudit(request, cancellationToken);

        TraceCreated("Audit", audit.Id);

        return CreatedAtRoute("GetAuditById", new { id = audit.Id }, audit);
    }
}