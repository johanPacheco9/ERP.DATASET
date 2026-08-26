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

        // Verificamos usando el DbSet correcto de tu entidad
        var existingSurplus = await _context.Set<UnidadProductoAuditada>()
            .AnyAsync(u =>
                    u.AuditId == request.AuditId &&
                    u.Serial == code &&
                    u.Status == UnitProductAuditStatus.ExcessProduct,
                cancellationToken);

        if (existingSurplus)
            throw new InvalidOperationException(
                $"El serial '{code}' ya fue registrado como sobrante en esta auditoria.");

        var productVariantExists = await _context.ProductoVariantes
            .AnyAsync(p =>
                    p.Id == request.ProductoVariantId.Value &&
                    p.ProductoBaseId == request.ProductId,
                cancellationToken);

        if (!productVariantExists)
            throw new InvalidOperationException("La variante seleccionada no pertenece a la linea de producto indicada.");

        if (audit.Status == AuditStatus.Pendiente)
            audit.Status = AuditStatus.InProgress;

        // Instanciamos la entidad exacta 'UnidadProductoAuditada'
        var surplusUnit = new UnidadProductoAuditada
        {
            AuditId = audit.Id,
            UnitProductId = 0, // Al ser un sobrante físico nuevo no vinculado previamente, se deja en 0 o se asocia
            ProductoBaseId = request.ProductId,
            ProductoVarianteId = request.ProductoVariantId.Value,
            BodegaId = audit.WarehouseId ?? request.PhysicalWarehouseId,
            BodegaEncontrada = request.PhysicalWarehouseId,
            Serial = code,
            Status = UnitProductAuditStatus.ExcessProduct,
            Observaciones = request.Observations,
            CreatedBy = request._AuditorAuth0Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Set<UnidadProductoAuditada>().Add(surplusUnit);
        audit.TotalSurplus++;

        await _context.SaveChangesAsync(cancellationToken);

        return new SurplusUnitDto(
            surplusUnit.Id,
            request.AuditId,
            code,
            request.ProductId,
            request.ProductoVariantId.Value,
            request.PhysicalWarehouseId,
            surplusUnit.UpdatedAt ?? DateTime.UtcNow,
            surplusUnit.CreatedBy);
    }
}