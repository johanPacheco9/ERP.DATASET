using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    public async Task<Result> RegisterScanAsync(int auditId, string code, CancellationToken cancellationToken = default)
    {
        code = code.Trim();

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
                if (auditDetail.Status == UnitProductAuditStatus.FoundOnAudit)
                {
                    return Result.Failure(new Error("Audit.AlreadyScanned",
                        $"La unidad '{code}' ya fue escaneada previamente en esta auditoría."));
                }

                auditDetail.Status = UnitProductAuditStatus.FoundOnAudit;
                auditDetail.UpdatedAt = DateTime.UtcNow;

                audit.TotalMatches += 1;
                audit.TotalCountedUnits += 1;
                audit.TotalMissing = audit.TotalExpectedUnits - audit.TotalMatches;

                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            else
            {
                // Escenario B: la unidad NO estaba planeada en esta auditoría (conteo ciego).
                // Buscar si corresponde a una unidad física real en el sistema.
                var unidadProducto = await context.UnidadesProductos
                    .Include(u => u.ProductoVariante)
                    .FirstOrDefaultAsync(u => u.SerialNumber == code, cancellationToken);

                if (unidadProducto == null)
                {
                    // Código no reconocido en el sistema: se rechaza, no se guarda huérfano.
                    return Result.Failure(new Error("Audit.UnknownSerial",
                        $"El código '{code}' no corresponde a ningún producto registrado en el sistema."));
                }

                if (unidadProducto.Status == UnidadProductoStatus.InAuditLock)
                {
                    // Ya está bloqueada por otro proceso de auditoría en curso (no esta).
                    return Result.Failure(new Error("Audit.AlreadyLocked",
                        $"La unidad '{code}' ya está bloqueada por otro proceso de auditoría en curso."));
                }

                var unexpectedUnit = new UnidadProductoAuditada
                {
                    AuditId = auditId,
                    UnitProductId = unidadProducto.Id,
                    ProductoBaseId = unidadProducto.ProductoVariante.ProductoBaseId,
                    ProductoVarianteId = unidadProducto.ProductoVarianteId,
                    BodegaId = audit.WarehouseId ?? 0,
                    BodegaEncontrada = unidadProducto.BodegaId,
                    Serial = code,
                    Status = UnitProductAuditStatus.ExcessProduct,
                    OriginalUnitStatus = unidadProducto.Status,
                    Observaciones = "Producto identificado, no estaba en el plan de esta auditoría.",
                    CreatedAt = DateTime.UtcNow
                };
                context.UnitProductAudits.Add(unexpectedUnit);

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