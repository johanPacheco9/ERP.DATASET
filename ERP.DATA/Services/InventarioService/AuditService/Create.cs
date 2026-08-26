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

        // 1. Apuntar a UnidadesProductos e incluir la relación con Variante y ProductoBase para los filtros
        var productsToAuditQuery = _context.UnidadesProductos
            .Include(u => u.ProductoVariante)
                .ThenInclude(v => v.ProductoBase)
            .Where(s => s.Status == UnidadProductoStatus.Available);

        if (request.IncludeReservedUnits)
        {
            productsToAuditQuery = _context.UnidadesProductos
                .Include(u => u.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
                .Where(s => s.Status == UnidadProductoStatus.Available ||
                            s.Status == UnidadProductoStatus.Separated);
        }

        if (request.WarehouseId.HasValue)
        {
            productsToAuditQuery = productsToAuditQuery
                .Where(u => u.BodegaId == request.WarehouseId.Value);
        }

        if (request.CategoryId.HasValue)
        {
            productsToAuditQuery = productsToAuditQuery
                .Where(u => u.ProductoVariante.ProductoBase.CategoryId == request.CategoryId.Value);
        }

        // Si tu request usa ProductId para referirse al ProductoBase
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
            // 2. Crear la cabecera de la Auditoría
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

            // 3. Mapear las líneas de la auditoría y congelar el stock de las unidades físicas
            var unitProductAudits = new List<UnidadProductoAuditada>();

            foreach (var unit in productsToAudit)
            {
                unitProductAudits.Add(new UnidadProductoAuditada
                {
                    AuditId = audit.Id,
                    UnitProductId = unit.Id,
                    ProductoVarianteId = unit.ProductoVarianteId,
                    BodegaId = unit.BodegaId,
                    Serial = unit.SerialNumber ?? unit.ProductoVariante.SKU,
                    Status = UnitProductAuditStatus.NotFound, // Inicia como no encontrado hasta que se escanee
                    CreatedBy = request._CreatorAuth0Id,
                    CreatedAt = DateTime.UtcNow
                });

                unit.Status = UnidadProductoStatus.InAuditLock; 
                unit.UpdatedAt = DateTime.UtcNow;
                unit.UpdatedBy = request._CreatorAuth0Id;
            }

            // Persistimos las líneas intermedias y los cambios de estado
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