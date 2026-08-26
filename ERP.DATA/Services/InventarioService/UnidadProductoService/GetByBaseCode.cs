using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.UnidadProductoService;

public partial class UnidadProductoManager
{
    public async Task<List<UnidadProductoDetailDto>> GetByBaseCode(
        GetUnitProductByCodeRequets request, 
        CancellationToken cancellationToken = default)
    {
        // 1. Proyectar directamente sobre UnidadesProductos cruzando la relación ProductoVariante -> ProductoBase
        var unidades = await context.UnidadesProductos
            .AsNoTracking()
            .Where(u => u.ProductoVariante.ProductoBase.Code == request.Code)
            .Select(u => new UnidadProductoDetailDto(
                u.Id,
                u.SerialNumber,
                u.Status,
                u.FechaVencimiento,
                u.ProductoVariante.ProductoBase.Name,
                u.ProductoVariante.ProductoBase.ImagenUrl,
                u.ProductoVariante.ProductoBase.Code,
                u.ProductoVariante.SKU,
                u.ProductoVariante.Atributos,
                u.ProductoVariante.PrecioVenta ?? u.ProductoVariante.ProductoBase.PrecioVenta, // Fallback al precio base si el SKU es null
                u.Bodega.Name
            ))
            .ToListAsync(cancellationToken);

        // 2. Validar disponibilidad de datos tras la ejecución
        if (unidades.Count == 0)
        {     
            throw new ArgumentException($"No se encontraron unidades físicas asociadas al código base '{request.Code}'.");
        }

        return unidades;
    }
}