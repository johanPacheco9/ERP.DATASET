using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.UnitProductService;

public partial class UnitProductService
{
    public async Task<PagedList<UnitProductDetailDto>> ListAsync(
        ListUnitProductRequest request,
        CancellationToken cancellationToken)
    {
        var query = context.Productos
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

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p =>
                (p.Serial != null && p.Serial.Contains(term)) ||
                p.SKU.Contains(term) ||
                p.LineaProducto.Name.Contains(term) ||
                p.LineaProducto.Code.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            var parts = request.OrderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = parts[0];
            var isDesc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = field.ToLowerInvariant() switch
            {
                "id" => isDesc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                "serial" => isDesc ? query.OrderByDescending(p => p.Serial) : query.OrderBy(p => p.Serial),
                "productname" => isDesc ? query.OrderByDescending(p => p.LineaProducto.Name) : query.OrderBy(p => p.LineaProducto.Name),
                "productocodigo" => isDesc ? query.OrderByDescending(p => p.LineaProducto.Code) : query.OrderBy(p => p.LineaProducto.Code),
                "precioventa" => isDesc ? query.OrderByDescending(p => p.PrecioVenta) : query.OrderBy(p => p.PrecioVenta),
                _ => query.OrderByDescending(p => p.Id)
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.Id);
        }

        var dtoQuery = query.Select(p => new UnitProductDetailDto(
            p.Id,
            p.Serial ?? p.SKU,
            p.Status,
            p.FechaVencimiento,
            p.LineaProducto.Name,
            p.LineaProducto.ImagenUrl,
            p.LineaProducto.Code,
            p.SKU,
            p.Atributos,
            p.PrecioVenta ?? p.LineaProducto.PrecioVenta,
            p.Bodega.Name ?? p.Bodega.Ubication ?? "Sin bodega"
        ));

        return await PagedList<UnitProductDetailDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }
}
