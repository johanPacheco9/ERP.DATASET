using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditsInventory;
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
            .AnyAsync(s => s.WarehouseId == warehouse.Id && s.Status != AuditStatus.Completada, cancellationToken);
        
        if (hasAuditInProgress)
        {
            throw new InvalidOperationException($"Ya hay una auditoría en progreso para la bodega requerida.");
        }

        var productsToAuditQuery = _context.Productos
            .Where(s => s.Status == ProductoStatus.Available);

        if (request.IncludeReservedUnits)
        {
            productsToAuditQuery = _context.Productos
                .Where(s => s.Status == ProductoStatus.Available ||
                            s.Status == ProductoStatus.Separated);
        }

        if (request.WarehouseId.HasValue)
        {
            productsToAuditQuery = productsToAuditQuery
                .Where(u => u.BodegaId == request.WarehouseId.Value);
        }

        if (request.CategoryId.HasValue)
        {
            productsToAuditQuery = productsToAuditQuery
                .Where(u => u.LineaProducto.CategoryId == request.CategoryId.Value);
        }

        if (request.ProductId.HasValue)
        {
            productsToAuditQuery = productsToAuditQuery
                .Where(u => u.LineaProductoId == request.ProductId.Value);
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
            // 1. Crear la cabecera de la Auditoría
            var audit = new Audit
            {
                StartDate = DateTime.UtcNow,
                EndDate = null,
                WarehouseId = request.WarehouseId,
                CategoryId = request.CategoryId,
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

            // 2. Mapear las líneas de la auditoría intermedia y congelar el stock principal
            var unitProductAudits = new List<UnitProductAudit>();

            foreach (var unit in productsToAudit)
            {
                // Generamos la línea intermedia
                unitProductAudits.Add(new UnitProductAudit
                {
                    AuditId = audit.Id,
                    UnitProductId = unit.Id,
                    LineaProductoId = unit.LineaProductoId,
                    ProductoId = unit.Id,
                    BodegaId = unit.BodegaId,
                    Serial = unit.Serial ?? unit.SKU,
                    Status = UnitProductAuditStatus.NotFound, // Inicia como no encontrado hasta que se pistolee
                    CreatedBy = request._CreatorAuth0Id,
                    CreatedAt = DateTime.UtcNow
                });
                unit.Status = ProductoStatus.InAuditLock; 
                unit.UpdatedAt = DateTime.UtcNow;
                unit.UpdatedBy = request._CreatorAuth0Id;
            }

            // Persistimos las líneas intermedias y los cambios de estado de los productos en un solo bloque
            _context.UnitProductAudits.AddRange(unitProductAudits);
            await _context.SaveChangesAsync(cancellationToken);

            // Confirmamos la transacción de forma segura
            await transaction.CommitAsync(cancellationToken);

            return new AuditDetailDto(
                audit.Id,
                audit.StartDate,
                audit.EndDate,
                audit.WarehouseId,
                warehouse.Name,
                audit.Category?.Name,
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
                audit.CreatedBy
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