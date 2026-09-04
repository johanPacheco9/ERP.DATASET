using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

/// <summary>
/// Servicio para crear una auditoria.
/// </summary>
public partial class AuditoriaService
{
    public async Task<AuditDetailDto> CreateAudit(
        CreateAuditRequest request,
        CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouse
            .FirstOrDefaultAsync(s => s.Id == request.WarehouseId, cancellationToken);
        
        if (warehouse == null)
        {
            throw new InvalidOperationException($"La bodega {request.WarehouseId} no existe.");
        }
        
        var hasAuditInProgress = await _context.Audit
            .AnyAsync(s => s.WarehouseId == warehouse.Id && (s.Status == AuditStatus.Pendiente || s.Status == AuditStatus.InProgress), cancellationToken);
        
        if (hasAuditInProgress)
        {
            throw new InvalidOperationException($"Ya hay una auditoría en progreso para la bodega requerida.");
        }
        
        var productsToAuditQuery = _context.UnidadesProductos
            .Include(u => u.ProductoVariante)
                .ThenInclude(v => v.ProductoBase)
                    .ThenInclude(pb => pb.Categorias)
            .Where(s => s.BodegaId == request.WarehouseId && 
                        (request.IncludeReservedUnits 
                            ? (s.Status == UnidadProductoStatus.Available || s.Status == UnidadProductoStatus.Separated) 
                            : s.Status == UnidadProductoStatus.Available));

        if (request.CategoryIds != null && request.CategoryIds.Any())
        {
            productsToAuditQuery = productsToAuditQuery
                .Where(u => u.ProductoVariante.ProductoBase.Categorias
                    .Any(pc => request.CategoryIds.Contains(pc.CategoryId)));
        }

        if (request.ProductId.HasValue)
        {
            productsToAuditQuery = productsToAuditQuery
                .Where(u => u.ProductoVariante.ProductoBaseId == request.ProductId.Value);
        }

        var productsToAudit = await productsToAuditQuery.ToListAsync(cancellationToken);

        if (!productsToAudit.Any())
        {
            throw new InvalidOperationException(
                "No se encontraron unidades para auditar con los filtros especificados.");
        }

        // Usamos una estrategia de transacción explícita para evitar bloqueos fantasma si falla el guardado intermedio
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 2. Crear la cabecera de la Auditoría (sin campos de categoría obsoletos)
            var audit = new Audit
            {
                StartDate = DateTime.UtcNow,
                EndDate = null,
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId,
                Type = request.Type,
                Status = AuditStatus.Pendiente,
                ResponsibleId = request.ResponsibleId,
                SupervisorId = request.SupervisorId,
                Observations = request.Observations,
                TotalExpectedUnits = productsToAudit.Count,
                TotalCountedUnits = 0,
                TotalMatches = 0,
                TotalMissing = 0,
                TotalSurplus = 0,
                TotalLocationDifferences = 0,
                TotalStatusDifferences = 0,
                CreatedBy = request._CreatorAuth0Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Audit.Add(audit);
            await _context.SaveChangesAsync(cancellationToken);

            // 3. Guardar la relación en la tabla intermedia de categorías auditadas
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var auditCategories = request.CategoryIds.Select(catId => new AuditCategory
                {
                    AuditId = audit.Id,
                    CategoryId = catId,
                    CreatedBy = request._CreatorAuth0Id,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                _context.Set<AuditCategory>().AddRange(auditCategories);
                await _context.SaveChangesAsync(cancellationToken);
            }
            
            var unitProductAudits = new List<UnidadProductoAuditada>();

            foreach (var unit in productsToAudit)
            {
                unitProductAudits.Add(new UnidadProductoAuditada
                {
                    AuditId = audit.Id,
                    UnitProductId = unit.Id,
                    ProductoVarianteId = unit.ProductoVarianteId,
                    ProductoBaseId = unit.ProductoVariante?.ProductoBaseId ?? 0, 
                    BodegaId = unit.BodegaId,
                    Serial = unit.SerialNumber ?? unit.ProductoVariante?.SKU ?? string.Empty,
                    Status = UnitProductAuditStatus.NotFound,
                    
                    OriginalUnitStatus = unit.Status,
        
                    CreatedBy = request._CreatorAuth0Id,
                    CreatedAt = DateTime.UtcNow
                });

                unit.Status = UnidadProductoStatus.InAuditLock; 
                unit.UpdatedAt = DateTime.UtcNow;
                unit.UpdatedBy = request._CreatorAuth0Id;
            }
            _context.UnitProductAudits.AddRange(unitProductAudits);
            await _context.SaveChangesAsync(cancellationToken);

            // Confirmamos la transacción
            await transaction.CommitAsync(cancellationToken);

            return new AuditDetailDto(
                audit.Id,
                audit.StartDate,
                audit.EndDate,
                audit.WarehouseId,
                warehouse.Name,
                null, // O un join formateado de las categorías desde la relación si tu DTO lo requiere
                audit.ProductId,
                audit.Product?.Name,
                audit.Type.GetDisplayName(),
                audit.Status.GetDisplayName(),
                audit.ResponsibleId,
                audit.SupervisorId,
                audit.TotalExpectedUnits,
                audit.TotalCountedUnits,
                audit.TotalMatches,
                audit.TotalMissing,
                audit.TotalSurplus,
                audit.TotalLocationDifferences,
                audit.TotalStatusDifferences,
                audit.Observations,
                audit.Conclusions,
                audit.CreatedAt,
                "Corregir cuando esté la relacion con user"
            );
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine($"Error crítico al crear auditoría: {e.Message}");
            throw;
        }
    }
}