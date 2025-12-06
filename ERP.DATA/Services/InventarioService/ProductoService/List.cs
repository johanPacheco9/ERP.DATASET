using ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.Inventario.ProductoService;

public partial class ProductoService
{
    public async Task<PagedList<ProductoSummaryDto>> ListAsync(
       ListProductRequest request,
       CancellationToken cancellationToken)
    {
        var query = _context.Productos
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            query = query.Where(p =>
                p.Nombre.Contains(request.OrderBy) ||
                p.Codigo.Contains(request.OrderBy) ||
                p.Categoria.Nombre.Contains(request.OrderBy) ||
                (p.Proveedor != null && p.Proveedor.Nombre.Contains(request.OrderBy))
            );
        }

        if (request.MinDate is not null)
            query = query.Where(p => p.CreatedAt >= request.MinDate.Value);

        if (request.MaxDate is not null)
            query = query.Where(p => p.CreatedAt <= request.MaxDate.Value);

        var dtoQuery = query
            .Select(p => new ProductoSummaryDto(
                p.Id,
                p.Nombre,
                p.Codigo,
                p.Descripcion,
                p.Precio_Venta,
                p.Costo_Unitario,
                p.Unidad_Medida,
                p.Es_Perecedero,
                p.Categoria.Nombre,
                p.Proveedor != null ? p.Proveedor.Nombre : null,
                p.Imagen_Url,
                p.Tags,
                p.Estado == ProductoEnumStatus.Activo
            ));

        return await PagedList<ProductoSummaryDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }

}