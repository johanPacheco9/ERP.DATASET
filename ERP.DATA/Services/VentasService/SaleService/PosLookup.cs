using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

public partial class SaleService
{
    public async Task<BarcodeLookupResultDto?> LookupProductByBarcodeAsync(
        string code,
        int warehouseId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var cleanCode = code.Trim();

        // 1. Buscar coincidencia exacta por Serial/Unidad Física disponible (UnidadProducto)
        var unidadFisica = await context.UnidadesProductos
            .AsNoTracking()
            .Include(u => u.ProductoVariante)
            .ThenInclude(v => v.ProductoBase)
            .ThenInclude(b => b.Categorias).ThenInclude(productoBaseCategory => productoBaseCategory.Category)
            .FirstOrDefaultAsync(u => u.Status == UnidadProductoStatus.Available &&
                                      u.BodegaId == warehouseId &&
                                      u.SerialNumber == cleanCode, cancellationToken);

        if (unidadFisica != null)
        {
            var variante = unidadFisica.ProductoVariante;
            var productoBase = variante.ProductoBase;

            // Consultar stock total disponible de la variante en la bodega
            var stockVariante = await context.WarehouseStock
                .Where(ws => ws.WarehouseId == warehouseId && ws.ProductoVarianteId == variante.Id)
                .Select(ws => ws.CurrentStock)
                .FirstOrDefaultAsync(cancellationToken);

            var taxRate = productoBase.ExentoIVA ? 0m : productoBase.PorcentajeIVA;
            var price = variante.PrecioVenta > 0 ? variante.PrecioVenta : productoBase.PrecioVenta;

            return new BarcodeLookupResultDto(
                productoBase.Id,              
                variante.Id,                   
                productoBase.Name,             
                productoBase.Code,             
                variante.SKU,                 
                variante.CodigoBarras ?? cleanCode,
                unidadFisica.SerialNumber,
                price ?? 0,                      
                taxRate,                   
                productoBase.ExentoIVA,       
                stockVariante,
                productoBase.ImagenUrl,
                productoBase.UnidadMedida ?? "UND",
                productoBase.Categorias?.Select(s=>new CategoriaDetailDto(s.Category.Id, s.Category.Name,s.Category.Description,s.Category.CreatedAt, s.Category.UpdatedAt)).ToList()
            );
        }

        // 2. Buscar por Código de Barras o SKU de la Variante (ProductoVariante)
        var varianteMatch = await context.ProductoVariantes
            .AsNoTracking()
            .Include(v => v.ProductoBase)
            .ThenInclude(b => b.Categorias).ThenInclude(productoBaseCategory => productoBaseCategory.Category)
            .FirstOrDefaultAsync(v => v.Status == ProductoVarianteStatus.Active &&
                                      (v.CodigoBarras == cleanCode || v.SKU == cleanCode), cancellationToken);

        if (varianteMatch != null)
        {
            var productoBase = varianteMatch.ProductoBase;

            var stockVariante = await context.WarehouseStock
                .Where(ws => ws.WarehouseId == warehouseId && ws.ProductoVarianteId == varianteMatch.Id)
                .Select(ws => ws.CurrentStock)
                .FirstOrDefaultAsync(cancellationToken);

            var taxRate = productoBase.ExentoIVA ? 0m : productoBase.PorcentajeIVA;
            var price = varianteMatch.PrecioVenta > 0 ? varianteMatch.PrecioVenta : productoBase.PrecioVenta;

            return new BarcodeLookupResultDto(
                productoBase.Id,               // LineaProductoId
                varianteMatch.Id,              // ProductoId
                productoBase.Name,             // Name
                productoBase.Code,             // Code
                varianteMatch.SKU,             // SKU
                varianteMatch.CodigoBarras ?? cleanCode, // CodigoBarras
                null,                          // Serial
                price ?? 0,                         // PrecioVenta
                taxRate,                       // PorcentajeIVA
                productoBase.ExentoIVA,        // ExentoIVA
                stockVariante,                 // AvailableStock
                productoBase.ImagenUrl,        // ImagenUrl
                productoBase.UnidadMedida ?? "UND", // UnidadMedida
                productoBase.Categorias?.Select(s=>new CategoriaDetailDto(s.Category.Id, s.Category.Name,s.Category.Description,s.Category.CreatedAt, s.Category.UpdatedAt)).ToList()   // Categoria
            );
        }

        return null;
    }

    public async Task<List<BarcodeLookupResultDto>> SearchProductsForPosAsync(
        string? query,
        List<int>? categoryIds,
        int warehouseId,
        int limit = 30,
        CancellationToken cancellationToken = default)
    {
        var variantesQuery = context.ProductoVariantes
            .AsNoTracking()
            .Include(v => v.ProductoBase)
                .ThenInclude(b => b.Categorias)
            .Where(v => v.Status == ProductoVarianteStatus.Active &&
                        v.ProductoBase.BaseStatus == ProductoBaseStatus.Active);

        if (categoryIds.Any() && categoryIds.Count > 0)
        {
            variantesQuery = variantesQuery.Where(v => v.ProductoBase.Categorias.Any(ct=> categoryIds.Contains(ct.CategoryId)));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var cleanQuery = query.Trim().ToLower();
            variantesQuery = variantesQuery.Where(v =>
                v.ProductoBase.Name.ToLower().Contains(cleanQuery) ||
                v.ProductoBase.Code.ToLower().Contains(cleanQuery) ||
                (v.SKU != null && v.SKU.ToLower().Contains(cleanQuery)) ||
                (v.CodigoBarras != null && v.CodigoBarras.ToLower().Contains(cleanQuery)));
        }

        var variantes = await variantesQuery
            .OrderBy(v => v.ProductoBase.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var varianteIds = variantes.Select(v => v.Id).ToList();

        // Cargar stock disponible de las variantes filtradas desde WarehouseStock
        var stockPorVariante = await context.WarehouseStock
            .Where(ws => ws.WarehouseId == warehouseId && varianteIds.Contains(ws.ProductoVarianteId))
            .ToDictionaryAsync(ws => ws.ProductoVarianteId, ws => ws.CurrentStock, cancellationToken);

        var result = new List<BarcodeLookupResultDto>();
        foreach (var v in variantes)
        {
            var b = v.ProductoBase;
            var stock = stockPorVariante.TryGetValue(v.Id, out var qty) ? qty : 0;
            var taxRate = b.ExentoIVA ? 0m : b.PorcentajeIVA;
            var price = v.PrecioVenta > 0 ? v.PrecioVenta : b.PrecioVenta;

            result.Add(new BarcodeLookupResultDto(
                b.Id,                  
                v.Id,                    
                b.Name,                  
                b.Code,                  
                v.SKU,                   
                v.CodigoBarras ?? b.Code,
                null,              
                price ?? 0,   
                taxRate,     
                b.ExentoIVA,             
                stock,                   
                b.ImagenUrl,             
                b.UnidadMedida ?? "UND",
                b.Categorias?.Select(s=>new CategoriaDetailDto(s.Category.Id, s.Category.Name,s.Category.Description,s.Category.CreatedAt, s.Category.UpdatedAt)).ToList() 
            ));
        }
        return result;
    }
}