using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.InventarioService.UnitProductService;

public partial class UnitProductService
{
    public async Task<UnitProductDetailDto?> GetById(int id, CancellationToken cancellation)
    {
        var result = await context.Productos
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UnitProductDetailDto(
                x.Id,
                x.Serial ?? x.SKU,
                x.Status,
                x.FechaVencimiento,
                x.LineaProducto.Name,
                x.LineaProducto.ImagenUrl,
                x.LineaProducto.Code,
                x.SKU,
                x.Atributos,
                x.PrecioVenta ?? x.LineaProducto.PrecioVenta,
                x.Bodega.Name ?? x.Bodega.Ubication ?? "Sin bodega"
            ))
            .FirstOrDefaultAsync(cancellation);

        if (result is null)
            logger.LogInformation("Producto (unidad) no encontrado. Id: {Id}", id);

        return result;
    }
}
