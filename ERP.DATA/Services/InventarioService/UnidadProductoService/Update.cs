using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.UnidadProductoService;

public partial class UnidadProductoManager
{
    public async Task<UnidadProductoDetailDto?> Update(UpdateUnitProductAuditRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id <= 0 || request.AuditId <= 0)
        {
            throw new ArgumentException("El Id de la unidad y el AuditId deben ser mayores a cero.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Buscamos el registro en la auditoría
            var unitAuditedProduct = await context.UnitProductAudits
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.AuditId == request.AuditId, cancellationToken);

            if (unitAuditedProduct == null)
            {
                return null;
            }

            // 2. Aplicamos las modificaciones del Request a la auditoría
            if (request.Status.HasValue)
            {
                var nuevoEstado = request.Status.Value;
                var estadoAnterior = unitAuditedProduct.Status;
                unitAuditedProduct.Status = nuevoEstado;

                // Si el estado cambia a una acción que afecta el inventario físico real
                if (nuevoEstado != estadoAnterior)
                {
                    // Buscamos la unidad de producto real asociada
                    var unidadReal = await context.UnidadesProductos
                        .FirstOrDefaultAsync(u => u.Id == unitAuditedProduct.UnitProductId, cancellationToken);

                    if (unidadReal != null)
                    {
                        var stockActual = await context.WarehouseStock
                            .FirstOrDefaultAsync(s => s.WarehouseId == unidadReal.BodegaId && s.ProductoVarianteId == unidadReal.ProductoVarianteId, cancellationToken);

                        switch (nuevoEstado)
                        {
                            case UnitProductAuditStatus.EnviadoABajas:
                            case UnitProductAuditStatus.SendToWritteOffWarehouse:
                                unidadReal.Status = UnidadProductoStatus.Damaged;
                                if (stockActual != null && stockActual.CurrentStock > 0)
                                {
                                    stockActual.CurrentStock -= 1;
                                }
                                break;

                            case UnitProductAuditStatus.EnviadoARecuperaciones:
                                unidadReal.Status = UnidadProductoStatus.Recovered;
                                break;

                            case UnitProductAuditStatus.NotFound:
                                unidadReal.Status = UnidadProductoStatus.Lost;
                                if (stockActual != null && stockActual.CurrentStock > 0)
                                {
                                    stockActual.CurrentStock -= 1;
                                }
                                break;

                            case UnitProductAuditStatus.Found:
                                unidadReal.Status = UnidadProductoStatus.Available;
                                if (stockActual != null)
                                {
                                    stockActual.CurrentStock += 1;
                                }
                                break;
                        }
                    }
                }
            }
                
            if (request.Observaciones != null)
                unitAuditedProduct.Observaciones = request.Observaciones;
                
            if (request.MotivoDiferencia != null)
                unitAuditedProduct.MotivoDiferencia = request.MotivoDiferencia;
                
            if (request.UbicacionFisica != null)
                unitAuditedProduct.UbicacionFisica = request.UbicacionFisica;
                
            if (request.EstadoFisico != null)
                unitAuditedProduct.EstadoFisico = request.EstadoFisico;
                
            if (request.RequiereAccionCorrectiva.HasValue)
                unitAuditedProduct.RequiereAccionCorrectiva = request.RequiereAccionCorrectiva.Value;

            unitAuditedProduct.UpdatedBy = request._UpdaterAuth0Id;
            unitAuditedProduct.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // 3. Proyección directa con LINQ para retornar el DTO actualizado
            var dto = await context.UnitProductAudits
                .AsNoTracking()
                .Where(u => u.Id == unitAuditedProduct.Id)
                .Select(u => new UnidadProductoDetailDto(
                    u.Id,
                    u.Serial ?? "SIN-SERIAL",
                    UnidadProductoStatus.Available, 
                    null, 
                    u.ProductoVariante != null && u.ProductoVariante.ProductoBase != null ? u.ProductoVariante.ProductoBase.Name : "Producto Desconocido",
                    u.ProductoVariante != null && u.ProductoVariante.ProductoBase != null ? u.ProductoVariante.ProductoBase.ImagenUrl : null,
                    u.ProductoVariante != null && u.ProductoVariante.ProductoBase != null ? u.ProductoVariante.ProductoBase.Code : null,
                    u.ProductoVariante != null ? u.ProductoVariante.SKU : null,
                    u.ProductoVariante != null ? u.ProductoVariante.Atributos : null, 
                    u.ProductoVariante != null ? (u.ProductoVariante.PrecioVenta ?? u.ProductoVariante.ProductoBase.PrecioVenta) : 0,
                    u.Bodega != null ? (u.Bodega.Name ?? u.Bodega.Ubication ?? "Sin bodega") : "Bodega No Asignada"
                ))
                .FirstOrDefaultAsync(cancellationToken);

            return dto;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine($"Error en UnitProductService.Update: {e.Message}");
            throw;
        }
    }
}