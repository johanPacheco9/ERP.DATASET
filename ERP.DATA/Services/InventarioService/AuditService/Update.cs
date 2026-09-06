using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    public async Task<bool> UpdateUnitProductAudit(UpdateUnitProductAuditRequest request, CancellationToken cancellationToken = default)
    {
        var unitAudit = await _context.UnitProductAudits
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (unitAudit == null)
            return false;

        var userId = await userManager.GetUserId();
        
        if (request.Status.HasValue)
            unitAudit.Status = request.Status.Value;
        if (request.Observaciones != null)
            unitAudit.Observaciones = request.Observaciones;
        if (request.MotivoDiferencia != null)
            unitAudit.MotivoDiferencia = request.MotivoDiferencia;
        if (request.UbicacionFisica != null)
            unitAudit.UbicacionFisica = request.UbicacionFisica;
        if (request.EstadoFisico != null)
            unitAudit.EstadoFisico = request.EstadoFisico;
        if (request.RequiereAccionCorrectiva.HasValue)
            unitAudit.RequiereAccionCorrectiva = request.RequiereAccionCorrectiva.Value;

        unitAudit.UpdatedBy = userId.Value;
        unitAudit.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
