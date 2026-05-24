using ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Services.InventarioService.ProductService;

public partial class ProductService
{
    public async Task<PagedList<ProductoSummaryDto>> ListAsync(
       ListProductRequest request,
       CancellationToken cancellationToken)
    {
        var query = context.LineaProductos
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = query.Where(p =>
                p.Name.Contains(request.OrderBy) ||
                p.Code.Contains(request.OrderBy) ||
                p.Categoria.Name.Contains(request.OrderBy) ||
                (p.Proveedor != null && p.Proveedor.Name.Contains(request.OrderBy))
            );
        }

        if (request.MinDate is not null)
            query = query.Where(p => p.CreatedAt >= request.MinDate.Value);

        if (request.MaxDate is not null)
            query = query.Where(p => p.CreatedAt <= request.MaxDate.Value);

        var dtoQuery = query
            .Select(p => new ProductoSummaryDto(
                p.Id,
                p.Name,
                p.Code,
                p.Description,
                p.PrecioVenta,
                p.CostoUnitario,
                p.UnidadMedida,
                p.EsPerecedero,
                p.Categoria.Name,
                p.Proveedor != null ? p.Proveedor.Name : null,
                p.ImagenUrl,
                p.Tags,
                p.Status == LineaProductoStatus.Active,
                p.Productos.Select(v => new ProductoVarianteDetailDto
                (
                    v.Id,
                    v.SKU,
                    v.Atributos,
                    v.PrecioVenta,
                    v.CostoUnitario,
                    p.Stock.Sum(s => s.CurrentStock), 
                    p.Stock.Sum(s => s.StockMinimo),
                    v.CodigoBarras,
                    v.IsActive
            )).ToList()
            ));

        return await PagedList<ProductoSummaryDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }

}