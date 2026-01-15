using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.UnitProductService;

public partial class UnitProductService
{
    public async Task<PagedList<UnitProductDetailDto>> ListAsync(
   ListUnitProductRequest request,
   CancellationToken cancellationToken)
    {
        var query = _context.UnitProduct
            .AsNoTracking()
            .AsQueryable();

        if (request.MinDate is not null)
        {
            var minUtc = DateTime.SpecifyKind(request.MinDate.Value, DateTimeKind.Utc);
            query = query.Where(p => p.CreatedAt >= minUtc);
        }

        if (request.MaxDate is not null)
        {
            var maxUtc = DateTime.SpecifyKind(request.MaxDate.Value, DateTimeKind.Utc);
            query = query.Where(p => p.CreatedAt <= maxUtc);
        }

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            var parts = request.OrderBy.Split(' ');
            var field = parts[0];
            var isDesc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = field.ToLower() switch
            {
                "id" => isDesc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                "productname" => isDesc ? query.OrderByDescending(p => p.Producto.Nombre) : query.OrderBy(p => p.Producto.Nombre),
                "productocodigo" => isDesc ? query.OrderByDescending(p => p.Producto.Codigo) : query.OrderBy(p => p.Producto.Codigo),
                "precioventa" => isDesc ? query.OrderByDescending(p => p.ProductoVariante.Precio_Venta) : query.OrderBy(p => p.ProductoVariante.Precio_Venta),
                _ => query.OrderByDescending(p => p.Id)
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.Id); // Default
        }

        var dtoQuery = query.Select(p => new UnitProductDetailDto(
            p.Id,
            p.Serial,
            p.UnitProductStatus.GetDisplayName(),
            p.FechaIngreso,
            p.Producto.Nombre,
            p.Producto.Imagen_Url,
            p.Producto.Codigo,
            p.ProductoVariante.CodigoVariante,
            p.ProductoVariante.Atributos,
            p.ProductoVariante.Precio_Venta,
            p.Bodega.Ubicacion ?? "No se tiene información"
        ));

        return await PagedList<UnitProductDetailDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }
}
