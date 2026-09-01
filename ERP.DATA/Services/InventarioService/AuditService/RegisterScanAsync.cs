using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    public async Task<Result> RegisterScanAsync(int auditId, string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var audit = await context.Audit
                .FirstOrDefaultAsync(a => a.Id == auditId, cancellationToken);

            if (audit == null)
                return Result.Failure(new Error("Audit.NotFound", "La auditoría especificada no existe."));

            if (audit.Status == AuditStatus.Completada)
                return Result.Failure(new Error("Audit.Closed",
                    "No se pueden escanear unidades en una auditoría cerrada."));

            if (audit.Status == AuditStatus.RejectWithinconsistences)
                return Result.Failure(new Error("Audit.Rejected",
                    "No se pueden escanear unidades en una auditoría rechazada."));

            // Primer escaneo de la auditoría: pasa de Pendiente a En progreso
            if (audit.Status == AuditStatus.Pendiente)
            {
                audit.Status = AuditStatus.InProgress;
            }

            // 1. Buscar si la unidad estaba planeada en esta auditoría
            var auditDetail = await context.UnitProductAudits
                .FirstOrDefaultAsync(d => d.AuditId == auditId && d.Serial == code, cancellationToken);

            if (auditDetail != null)
            {
                // Escenario A: La unidad SÍ pertenecía a la auditoría
                if (auditDetail.Status == UnitProductAuditStatus.Found)
                {
                    return Result.Failure(new Error("Audit.AlreadyScanned",
                        $"La unidad '{code}' ya fue escaneada previamente en esta auditoría."));
                }

                auditDetail.Status = UnitProductAuditStatus.Found;
                auditDetail.UpdatedAt = DateTime.UtcNow;

                audit.TotalMatches += 1;
                audit.TotalCountedUnits += 1;
                audit.TotalMissing = audit.TotalExpectedUnits - audit.TotalMatches;

                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            else
            {
                // Escenario B: La unidad NO estaba listada -> Es un Sobrante (ExcessProduct = 60)
                // Queda sin producto identificado hasta completarse desde el modal "Registrar Sobrante".
                // El cierre de la auditoría bloquea mientras existan sobrantes sin identificar.
                var excessUnit = new UnidadProductoAuditada
                {
                    AuditId = auditId,
                    Serial = code,
                    BodegaId = audit.WarehouseId ?? 0,
                    Status = UnitProductAuditStatus.ExcessProduct, // 60
                    Observaciones = "Detectado y registrado automáticamente como producto en exceso por escaneo. Pendiente de identificar producto.",
                    CreatedAt = DateTime.UtcNow
                };
                context.UnitProductAudits.Add(excessUnit);

                audit.TotalSurplus += 1;
                audit.TotalCountedUnits += 1;

                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Audit.ScanError", $"Error al registrar el escaneo: {ex.Message}"));
        }
    }
}