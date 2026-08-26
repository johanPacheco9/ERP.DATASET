using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.UnidadProductoService;

public partial class UnidadProductoManager
{
    public async Task<UnidadProductoDetailDto?> GetById(int id, CancellationToken cancellation)
    {
        var result = await context.UnidadesProductos.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UnidadProductoDetailDto(
                x.Id,
                x.SerialNumber ?? x.ProductoVariante.SKU, // Ajustado a SerialNumber si ese es el nombre en la entidad
                x.Status,
                x.FechaVencimiento,
                x.ProductoVariante.ProductoBase.Name,
                x.ProductoVariante.ProductoBase.ImagenUrl,
                x.ProductoVariante.ProductoBase.Code,
                x.ProductoVariante.SKU,
                x.ProductoVariante.Atributos,
                x.ProductoVariante.PrecioVenta ?? x.ProductoVariante.PrecioVenta ?? x.ProductoVariante.ProductoBase.PrecioVenta,
                x.Bodega.Name ?? x.Bodega.Ubication ?? "Sin bodega"
            ))
            .FirstOrDefaultAsync(cancellation);

        if (result is null)
            logger.LogInformation("Producto (unidad) no encontrado. Id: {Id}", id);

        return result;
    }
}