using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.ProductoBaseService;

public partial class ProductoBaseService
{
    public async Task<PagedList<ProductoSummaryDto>> ListAsync(
        ListProductRequest request,
        string? searchTerm,
        string? categoryName,
        string? stockFilter,
        CancellationToken cancellationToken)
    {
        var query = context.ProductoBase
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Include(p => p.Variantes) 
                .ThenInclude(v => v.Stocks) 
            .AsQueryable();

        // 1. Filtro por Búsqueda (Nombre, Código, Categoría o Proveedor)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Code.Contains(searchTerm) ||
                p.Categoria.Name.Contains(searchTerm) ||
                (p.Proveedor != null && p.Proveedor.Name.Contains(searchTerm))
            );
        }

        // 2. Filtro por Categoría
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(p => p.Categoria.Name == categoryName);
        }

        // 3. Filtro por Estado de Stock (Evaluado usando Variantes y su respectivo Stock)
        if (stockFilter == "low")
        {
            query = query.Where(p => p.Variantes.SelectMany(v => v.Stocks).Sum(s => s.CurrentStock) < p.Variantes.SelectMany(v => v.Stocks).Sum(s => s.StockMinimo));
        }
        else if (stockFilter == "normal")
        {
            query = query.Where(p => p.Variantes.SelectMany(v => v.Stocks).Sum(s => s.CurrentStock) >= p.Variantes.SelectMany(v => v.Stocks).Sum(s => s.StockMinimo));
        }

        // 4. Mapeo a DTO respetando la estructura ProductoBase -> Variantes -> Stock
        var dtoQuery = query.Select(p => new ProductoSummaryDto(
            p.Id,
            p.Name,
            p.Code,
            p.Description,
            p.PrecioVenta,
            p.CostoUnitario,
            p.UnidadMedida,
            p.EsPerecedero,
            p.Categoria != null ? p.Categoria.Name : "Sin Categoría",
            p.Proveedor != null ? p.Proveedor.Name : null,
            p.ImagenUrl,
            p.Tags,
            p.BaseStatus == ProductoBaseStatus.Active,
            p.Variantes != null ? p.Variantes.Select(v => new ProductoVarianteDetailDto(
                v.Id,
                v.SKU,
                v.Atributos,
                v.PrecioVenta,
                v.CostoUnitario,
                v.Stocks != null ? v.Stocks.Sum(s => s.CurrentStock) : 0,        // Stock específico de esta variante
                v.Stocks != null ? v.Stocks.Sum(s => s.StockMinimo) : 0,    // Stock mínimo específico de esta variante
                v.CodigoBarras,
                v.IsActive
            )).ToList() : new List<ProductoVarianteDetailDto>()
        ));
        
        // 5. Paginación en Base de Datos (Corregido: sin pasar el token si el método no lo soporta)
        return await PagedList<ProductoSummaryDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }
}