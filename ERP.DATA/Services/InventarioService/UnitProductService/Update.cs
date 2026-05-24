using ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.UnitProductService;

public partial class UnitProductService
{
    public async Task<UnitProductDetailDto?> Update(UpdateUnitProductAuditRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Id <= 0 || request.AuditId <= 0)
        {
            throw new ArgumentException("El Id de la unidad y el AuditId deben ser mayores a cero.");
        }

        try
        {
            // 1. Buscamos el registro en la tabla intermedia de la auditoría
            var unitAuditedProduct = await context.UnitProductAudits
                .FirstOrDefaultAsync(s => s.Id == request.Id && s.AuditId == request.AuditId, cancellationToken);

            if (unitAuditedProduct == null)
            {
                return null;
            }

            // 2. Aplicamos las modificaciones del Request (Campos mutables de la auditoría)
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

            // Persistimos los cambios en la base de datos
            await context.SaveChangesAsync(cancellationToken);

            // 3. Consultamos y proyectamos las relaciones para construir tu UnitProductDetailDto estricto
            // Usamos AsNoTracking porque es una proyección de pura lectura para la vista
            var dto = await context.UnitProductAudits
                .AsNoTracking()
                .Where(u => u.Id == unitAuditedProduct.Id)
                .Select(u => new UnitProductDetailDto(
                    u.Id,
                    u.Serial ?? "SIN-SERIAL",
                    u.Producto != null ? u.Producto.Status : ProductoStatus.Available, // Ajusta según tu lógica si es null
                    u.Producto != null ? u.Producto.CreatedAt : null, // Mapea la fecha correspondiente a tu modelo física
                    u.Producto != null ? u.Producto.LineaProducto.Name : "Producto Desconocido",
                    u.Producto != null ? u.Producto.LineaProducto.ImagenUrl : null,
                    u.Producto != null ? u.Producto.LineaProducto.Code : null,
                    u.Producto != null ? u.Producto.SKU : null, // O el campo del código de variante que manejes
                    u.Observaciones, // Mapeado al campo string? Atributos temporalmente o los detalles físicos
                    u.Producto != null ? u.Producto.LineaProducto.PrecioVenta : 0, 
                    u.Bodega != null ? u.Bodega.Name : "Bodega No Asignada"
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
