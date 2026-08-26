using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.UnidadProductoService;

public partial class UnidadProductoManager
{
    public async Task<PagedList<UnidadProductoDetailDto>> ListAsync(
        ListUnitProductRequest request,
        CancellationToken cancellationToken)
    {
        var query = context.UnidadesProductos.AsNoTracking()
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
                (p.SerialNumber.Contains(term)) ||
                p.ProductoVariante.SKU.Contains(term) ||
                p.ProductoVariante.ProductoBase.Name.Contains(term) ||
                p.ProductoVariante.ProductoBase.Code.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            var parts = request.OrderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = parts[0];
            var isDesc = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = field.ToLowerInvariant() switch
            {
                "id" => isDesc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
                "serial" => isDesc ? query.OrderByDescending(p => p.SerialNumber) : query.OrderBy(p => p.SerialNumber),
                "productname" => isDesc ? query.OrderByDescending(p => p.ProductoVariante.ProductoBase.Name) : query.OrderBy(p => p.ProductoVariante.ProductoBase.Name),
                "productocodigo" => isDesc ? query.OrderByDescending(p => p.ProductoVariante.ProductoBase.Code) : query.OrderBy(p => p.ProductoVariante.ProductoBase.Code),
                "precioventa" => isDesc ? query.OrderByDescending(p => p.ProductoVariante.PrecioVenta ?? p.ProductoVariante.PrecioVenta ?? p.ProductoVariante.ProductoBase.PrecioVenta) : query.OrderBy(p => p.ProductoVariante.PrecioVenta ?? p.ProductoVariante.PrecioVenta ?? p.ProductoVariante.ProductoBase.PrecioVenta),
                _ => query.OrderByDescending(p => p.Id)
            };
        }
        else
        {
            query = query.OrderByDescending(p => p.Id);
        }

        var dtoQuery = query.Select(p => new UnidadProductoDetailDto(
            p.Id,
            p.SerialNumber ?? p.ProductoVariante.SKU,
            p.Status,
            p.FechaVencimiento,
            p.ProductoVariante.ProductoBase.Name,
            p.ProductoVariante.ProductoBase.ImagenUrl,
            p.ProductoVariante.ProductoBase.Code,
            p.ProductoVariante.SKU,
            p.ProductoVariante.Atributos,
            p.ProductoVariante.PrecioVenta ?? p.ProductoVariante.PrecioVenta ?? p.ProductoVariante.ProductoBase.PrecioVenta,
            p.Bodega.Name ?? p.Bodega.Ubication ?? "Sin bodega"
        ));

        return await PagedList<UnidadProductoDetailDto>.ToPagedListAsync(
            dtoQuery,
            request.PageNumber,
            request.PageSize
        );
    }
}