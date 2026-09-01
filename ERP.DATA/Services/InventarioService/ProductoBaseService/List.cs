using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Responses;
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
            .Include(p => p.Categorias)
            .Include(p => p.Marca)
            .Include(p => p.Proveedores)
                .ThenInclude(pp => pp.Proveedor)
            .Include(p => p.Variantes)
                .ThenInclude(v => v.Stocks)
            .AsQueryable();

        // 1. Filtro por búsqueda
        // Busca por Nombre, Código, Categoría, Marca o Proveedor.
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(searchTerm) ||
                p.Code.Contains(searchTerm) ||
                p.Categorias.Any(s => s.Category.Name.Contains(searchTerm)) ||
                p.Marca.Nombre.Contains(searchTerm) ||
                p.Proveedores.Any(pp =>
                    pp.Proveedor.Name.Contains(searchTerm))
            );
        }
        
        // 2. Filtro por categoría
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(p =>
                p.Categorias.Any(s => s.Category.Name == categoryName));
        }

        // 3. Filtro por estado de stock
        if (stockFilter == "low")
        {
            query = query.Where(p =>
                p.Variantes
                    .SelectMany(v => v.Stocks)
                    .Sum(s => s.CurrentStock)
                <
                p.Variantes
                    .SelectMany(v => v.Stocks)
                    .Sum(s => s.StockMinimo));
        }
        else if (stockFilter == "normal")
        {
            query = query.Where(p =>
                p.Variantes
                    .SelectMany(v => v.Stocks)
                    .Sum(s => s.CurrentStock)
                >=
                p.Variantes
                    .SelectMany(v => v.Stocks)
                    .Sum(s => s.StockMinimo));
        }

        // 4. Proyección a DTO
        var dtoQuery = query.Select(p => new ProductoSummaryDto(
            p.Id,
            p.Name,
            p.Code,
            p.Description,
            p.PrecioVenta,
            p.CostoUnitario,
            p.UnidadMedida,
            p.EsPerecedero,
            // Categoría
            p.Categorias.Select(s=> new CategoryDto(s.Category.Id,s.Category.Name)).ToList(),
            // Marca
            p.Marca != null
                ? p.Marca.Nombre
                : "Sin Marca",
            p.Proveedores
                .Select(pp => pp.Proveedor.Name)
                .ToList(),
            p.ImagenUrl,
            p.Tags,
            p.BaseStatus == ProductoBaseStatus.Active,
            // Variantes
            p.Variantes != null
                ? p.Variantes.Select(v => new ProductoVarianteDetailDto(
                    v.Id,
                    v.SKU,
                    v.Atributos,
                    v.PrecioVenta,
                    v.CostoUnitario,

                    // Stock actual
                    v.Stocks != null
                        ? v.Stocks.Sum(s => s.CurrentStock)
                        : 0,

                    // Stock mínimo
                    v.Stocks != null
                        ? v.Stocks.Sum(s => s.StockMinimo)
                        : 0,

                    v.CodigoBarras,
                    v.IsActive
                )).ToList()
                : new List<ProductoVarianteDetailDto>()
        ));

        // 5. Paginación
        return await PagedList<ProductoSummaryDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }
}

