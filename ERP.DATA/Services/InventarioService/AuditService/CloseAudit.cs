using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.AuditService;

public partial class AuditoriaService
{
    /// <summary>
    /// Cierra la auditoría, calcula los totales finales, libera el inventario bloqueado,
    /// registra faltantes como pérdida, incorpora sobrantes al inventario y guarda las conclusiones.
    /// </summary>
    public async Task<AuditDetailDto> CloseAudit(
        CloseAuditRequest request,
        CancellationToken cancellationToken)
    {
        var audit = await _context.Audit
            .Include(a => a.Warehouse)
            .Include(a => a.CategoriasAuditadas).ThenInclude(auditCategory => auditCategory.Category)
            .Include(a => a.Product)
            .FirstOrDefaultAsync(a => a.Id == request.AuditId, cancellationToken);

        if (audit == null)
            throw new InvalidOperationException($"La auditoría {request.AuditId} no existe.");

        if (audit.Status == AuditStatus.Completada)
            throw new InvalidOperationException("La auditoría ya fue cerrada.");

        if (audit.Status == AuditStatus.RejectWithinconsistences)
            throw new InvalidOperationException("No se puede cerrar una auditoría cancelada.");

        // 1. Obtener los detalles de la auditoría
        var unitAudits = await _context.UnitProductAudits
            .Where(u => u.AuditId == request.AuditId)
            .ToListAsync(cancellationToken);

        // Guardrail: no cerrar con sobrantes sin producto identificado
        var sobrantesSinIdentificar = unitAudits
            .Count(u => u.Status == UnitProductAuditStatus.ExcessProduct && u.ProductoVarianteId == 0);

        if (sobrantesSinIdentificar > 0)
        {
            throw new InvalidOperationException(
                $"Hay {sobrantesSinIdentificar} unidad(es) sobrante(s) sin producto identificado. " +
                "Complételas desde el modal de sobrantes antes de cerrar la auditoría.");
        }

        // 2. Obtener las unidades físicas ya existentes (encontradas y faltantes; los sobrantes aún no existen)
        var unitProductIds = unitAudits
            .Where(u => u.Status != UnitProductAuditStatus.ExcessProduct)
            .Select(u => u.UnitProductId)
            .ToList();

        var physicalUnits = await _context.UnidadesProductos
            .Where(u => unitProductIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        var bodegaAuditoriaId = audit.WarehouseId ?? 0;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var faltantes = unitAudits.Where(u => u.Status == UnitProductAuditStatus.NotFound).ToList();
            var sobrantes = unitAudits.Where(u => u.Status == UnitProductAuditStatus.ExcessProduct).ToList();

            // 3. Liberar unidades bloqueadas: recuperan su estado original o pasan a Lost si no se encontraron
            foreach (var physicalUnit in physicalUnits)
            {
                if (physicalUnit.Status != UnidadProductoStatus.InAuditLock) continue;

                var auditLine = unitAudits.FirstOrDefault(a => a.UnitProductId == physicalUnit.Id);

                physicalUnit.Status = auditLine?.Status == UnitProductAuditStatus.NotFound
                    ? UnidadProductoStatus.Lost
                    : auditLine?.OriginalUnitStatus ?? UnidadProductoStatus.Available;

                physicalUnit.UpdatedAt = DateTime.UtcNow;
                physicalUnit.UpdatedBy = request._CloserAuth0Id;
            }

            // 4. Faltantes: descontar stock y dejar constancia del movimiento de pérdida
            if (faltantes.Count > 0)
            {
                var movimientoPerdida = new Movement
                {
                    AuditId = audit.Id,
                    OrigenWarehouseId = bodegaAuditoriaId,
                    DestinationWarehouseId = null,
                    Type = TipoMovimiento.Perdida,
                    Quantity = faltantes.Count,
                    UnitCost = 0,
                    Observations = request.Conclusions,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request._CloserAuth0Id
                };
                _context.Movements.Add(movimientoPerdida);

                var variantIdsFaltantes = physicalUnits
                    .Where(u => faltantes.Any(f => f.UnitProductId == u.Id))
                    .Select(u => u.ProductoVarianteId)
                    .Distinct()
                    .ToList();

                var stockFaltantes = await _context.WarehouseStock
                    .Where(s => s.WarehouseId == bodegaAuditoriaId && variantIdsFaltantes.Contains(s.ProductoVarianteId))
                    .ToListAsync(cancellationToken);

                // Agrupamos el descuento total por variante para evitar restar unidad por unidad de forma repetitiva
                var faltantesPorVariante = physicalUnits
                    .Where(u => faltantes.Any(f => f.UnitProductId == u.Id))
                    .GroupBy(u => u.ProductoVarianteId);

                foreach (var grupo in faltantesPorVariante)
                {
                    var stockOrigen = stockFaltantes.FirstOrDefault(s => s.ProductoVarianteId == grupo.Key);
                    if (stockOrigen != null)
                    {
                        stockOrigen.CurrentStock -= grupo.Count();
                    }
                }

                foreach (var detalle in faltantes)
                {
                    var unidad = physicalUnits.FirstOrDefault(u => u.Id == detalle.UnitProductId);
                    if (unidad is null) continue;

                    _context.UnitProductMovements.Add(new UnitProductMovement
                    {
                        UnidadProductoId = unidad.Id,
                        Movimiento = movimientoPerdida, // Vinculación por navegación en memoria
                        TipoMovimiento = TipoMovimiento.Perdida,
                        BodegaOrigenId = bodegaAuditoriaId,
                        BodegaDestinoId = null,
                        Motivo = "Faltante detectado en auditoría de inventario",
                        Observaciones = detalle.Observaciones
                    });
                }
            }

            // 5. Sobrantes: crear la unidad física, sumar stock y dejar constancia del movimiento de entrada
            if (sobrantes.Count > 0)
            {
                var movimientoEntrada = new Movement
                {
                    AuditId = audit.Id,
                    OrigenWarehouseId = bodegaAuditoriaId,
                    DestinationWarehouseId = null,
                    Type = TipoMovimiento.Entrada,
                    Quantity = sobrantes.Count,
                    UnitCost = 0,
                    Observations = request.Conclusions,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request._CloserAuth0Id
                };
                _context.Movements.Add(movimientoEntrada);

                var variantIdsSobrantes = sobrantes.Select(d => d.ProductoVarianteId).Distinct().ToList();
                var stockSobrantes = await _context.WarehouseStock
                    .Where(s => s.WarehouseId == bodegaAuditoriaId && variantIdsSobrantes.Contains(s.ProductoVarianteId))
                    .ToListAsync(cancellationToken);

                foreach (var detalle in sobrantes)
                {
                    var nuevaUnidad = new UnidadProducto
                    {
                        BodegaId = bodegaAuditoriaId,
                        ProductoVarianteId = detalle.ProductoVarianteId,
                        SerialNumber = detalle.Serial,
                        Status = UnidadProductoStatus.Available,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = request._CloserAuth0Id
                    };
                    _context.UnidadesProductos.Add(nuevaUnidad);

                    _context.UnitProductMovements.Add(new UnitProductMovement
                    {
                        UnidadProductoId = nuevaUnidad.Id,
                        Movimiento = movimientoEntrada, // Vinculación por navegación en memoria
                        TipoMovimiento = TipoMovimiento.Entrada,
                        BodegaOrigenId = bodegaAuditoriaId,
                        BodegaDestinoId = null,
                        Motivo = "Sobrante detectado en auditoría de inventario",
                        Observaciones = detalle.Observaciones
                    });

                    var stockDestino = stockSobrantes.FirstOrDefault(s => s.ProductoVarianteId == detalle.ProductoVarianteId);
                    if (stockDestino != null)
                    {
                        stockDestino.CurrentStock += 1;
                    }
                    else
                    {
                        stockDestino = new WarehouseStock
                        {
                            WarehouseId = bodegaAuditoriaId,
                            ProductoVarianteId = detalle.ProductoVarianteId,
                            CurrentStock = 1
                        };
                        _context.WarehouseStock.Add(stockDestino);
                        stockSobrantes.Add(stockDestino);
                    }
                }
            }

            // 6. Totales finales de la cabecera
            audit.TotalExpectedUnits = unitAudits.Count(u => u.Status != UnitProductAuditStatus.ExcessProduct);
            audit.TotalCountedUnits = unitAudits.Count(u => u.Status != UnitProductAuditStatus.NotFound);
            audit.TotalMatches = unitAudits.Count(u => u.Status == UnitProductAuditStatus.Found);
            audit.TotalMissing = faltantes.Count;
            audit.TotalSurplus = sobrantes.Count;
            audit.TotalLocationDifferences = 0; // Not supported by enum
            audit.TotalStatusDifferences = unitAudits.Count(u => u.Status == UnitProductAuditStatus.StatusMismatch);

            // Asignación de estado final condicional sin sobrescrituras erróneas
            if (faltantes.Count > 0 || sobrantes.Count > 0 || audit.TotalLocationDifferences > 0 || audit.TotalStatusDifferences > 0)
            {
                audit.Status = AuditStatus.ClosedWithInconsistences;
            }
            else
            {
                audit.Status = AuditStatus.Completada;
            }

            audit.EndDate = DateTime.UtcNow;
            audit.Conclusions = request.Conclusions;
            audit.UpdatedAt = DateTime.UtcNow;
            audit.UpdatedBy = request._CloserAuth0Id;

            // Único SaveChanges al final de toda la lógica transaccional
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // Mapear la lista de categorías desde audit.CategoriasAuditadas
            var categoriasDto = audit.CategoriasAuditadas?
                .Select(ac => new CategoriaDetailDto(
                    ac.Category.Id,
                    ac.Category.Name,
                    ac.Category.Description,
                    ac.Category.CreatedAt,
                    ac.Category.UpdatedAt
                ))
                .ToList() ?? new List<CategoriaDetailDto>();

            return new AuditDetailDto(
                audit.Id,
                audit.StartDate,
                audit.EndDate,
                audit.WarehouseId,
                audit.Warehouse?.Name,
                categoriasDto,
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
                "Corregir creado por"
            );
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}