using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
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

        try
        {
            // 1. Buscamos y actualizamos el registro directamente (sin Include innecesarios)
            var unitAuditedProduct = await context.UnitProductAudits
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.AuditId == request.AuditId, cancellationToken);

            if (unitAuditedProduct == null)
            {
                return null;
            }

            // 2. Aplicamos las modificaciones del Request
            if (request.Status.HasValue)
                unitAuditedProduct.Status = request.Status.Value;
                
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

            // 3. Proyección directa con LINQ (EF Core maneja los JOINs automáticamente sin requerir .Include())
            var dto = await context.UnitProductAudits
                .AsNoTracking()
                .Where(u => u.Id == unitAuditedProduct.Id)
                .Select(u => new UnidadProductoDetailDto(
                    u.Id,
                    u.Serial ?? "SIN-SERIAL",
                    UnidadProductoStatus.Available, // Ajusta aquí al estado real de la unidad auditada según tu lógica de negocio
                    null, // FechaVencimiento si aplica desde otra relación
                    u.ProductoVariante != null && u.ProductoVariante.ProductoBase != null ? u.ProductoVariante.ProductoBase.Name : "Producto Desconocido",
                    u.ProductoVariante != null && u.ProductoVariante.ProductoBase != null ? u.ProductoVariante.ProductoBase.ImagenUrl : null,
                    u.ProductoVariante != null && u.ProductoVariante.ProductoBase != null ? u.ProductoVariante.ProductoBase.Code : null,
                    u.ProductoVariante != null ? u.ProductoVariante.SKU : null,
                    u.ProductoVariante != null ? u.ProductoVariante.Atributos : null, // Mapeado correctamente al atributo Atributos del DTO
                    u.ProductoVariante != null ? (u.ProductoVariante.PrecioVenta ?? u.ProductoVariante.ProductoBase.PrecioVenta) : 0,
                    u.Bodega != null ? (u.Bodega.Name ?? u.Bodega.Ubication ?? "Sin bodega") : "Bodega No Asignada"
                ))
                .FirstOrDefaultAsync(cancellationToken);

            return dto;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error en UnitProductService.Update: {e.Message}");
            throw;
        }
    }
}