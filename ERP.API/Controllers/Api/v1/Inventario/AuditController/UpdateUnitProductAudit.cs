using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
namespace ERP.API.Controllers.Api.v1.Inventario.AuditController;

using ERP.TRAN.CrossLayers.API.Inventario.Audit;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IAudit;
using Microsoft.AspNetCore.Mvc;

public sealed class UpdateUnitProductAuditEndpoint : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<UpdateUnitProductAuditEndpoint> _logger;

    public UpdateUnitProductAuditEndpoint(ILogger<UpdateUnitProductAuditEndpoint> logger, IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    [Tags("Inventory - Audits")]
    [HttpPatch(AuditEndpoints.UnitProductAuditById, Name = "UpdateUnitProductAudit")]
    public async Task<ActionResult> HandleAsync(
        [FromRoute] int id,
        [FromBody] UpdateUnitProductAuditRequest request,
        CancellationToken cancellationToken = default)
    {
        request.Id = id;

        var updated = await _auditService.UpdateUnitProductAudit(request, cancellationToken);

        if (!updated)
            return NotFound(new { message = $"No se encontró UnitProductAudit con id {id}" });

        return Ok(new { message = "UnitProductAudit actualizado correctamente" });
    }
}
