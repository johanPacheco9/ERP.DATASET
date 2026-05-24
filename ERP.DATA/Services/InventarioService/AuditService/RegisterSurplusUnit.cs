using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    /// <summary>
    /// Registra una unidad fisica encontrada en bodega que no existe en esta auditoria.
    /// </summary>
    public async Task<SurplusUnitDto> RegisterSurplusUnit(
        RegisterSurplusUnitRequest request,
        CancellationToken cancellationToken)
    {
        var audit = await _context.Audit
            .FirstOrDefaultAsync(a => a.Id == request.AuditId, cancellationToken);

        if (audit == null)
            throw new InvalidOperationException($"La auditoria {request.AuditId} no existe.");

        if (audit.Status == AuditStatus.Completada || audit.Status == AuditStatus.RejectWithinconsistences)
            throw new InvalidOperationException(
                $"No se pueden registrar sobrantes en una auditoria con estado '{audit.Status.GetDisplayName()}'.");

        if (string.IsNullOrWhiteSpace(request.Code))
            throw new InvalidOperationException("Debe indicar el serial o codigo fisico del sobrante.");

        if (request.ProductId <= 0 || request.ProductoVariantId is null or <= 0)
            throw new InvalidOperationException("Debe seleccionar una linea de producto y una variante para registrar el sobrante.");

        var code = request.Code.Trim();

        var existingSurplus = await _context.UnitProductAudits
            .AnyAsync(u =>
                    u.AuditId == request.AuditId &&
                    u.Serial == code &&
                    u.Status == UnitProductAuditStatus.ExcessProduct,
                cancellationToken);

        if (existingSurplus)
            throw new InvalidOperationException(
                $"El serial '{code}' ya fue registrado como sobrante en esta auditoria.");

        var productVariantExists = await _context.Productos
            .AnyAsync(p =>
                    p.Id == request.ProductoVariantId.Value &&
                    p.LineaProductoId == request.ProductId,
                cancellationToken);

        if (!productVariantExists)
            throw new InvalidOperationException("La variante seleccionada no pertenece a la linea de producto indicada.");

        if (audit.Status == AuditStatus.Pendiente)
            audit.Status = AuditStatus.InProgress;

        var surplusUnit = new UnitProductAudit
        {
            AuditId = audit.Id,
            UnitProductId = 0,
            LineaProductoId = request.ProductId,
            ProductoId = request.ProductoVariantId.Value,
            BodegaId = request.PhysicalWarehouseId,
            BodegaEncontrada = request.PhysicalWarehouseId,
            Serial = code,
            Status = UnitProductAuditStatus.ExcessProduct,
            UpdatedAt = DateTime.UtcNow,
            Observaciones = request.Observations,
            CreatedBy = request._AuditorAuth0Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.UnitProductAudits.Add(surplusUnit);
        audit.TotalSurplus++;

        await _context.SaveChangesAsync(cancellationToken);

        return new SurplusUnitDto(
            surplusUnit.Id,
            request.AuditId,
            code,
            request.ProductId,
            request.ProductoVariantId,
            request.PhysicalWarehouseId,
            surplusUnit.UpdatedAt,
            surplusUnit.CreatedBy);
    }
}
