using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.MovimientoService;

public partial class MovimientoService
{
    public async Task<List<MovimientoDetailDto>> ListMovements(ListMovementsRequest request)
    {
        // 1. Iniciamos la consulta base
        var query = context.Movements.AsNoTracking();

        // 2. Aplicamos filtros opcionales (StoreId y Fechas)
        if (request.StoreId.HasValue)
        {
            // Asumiendo que la entidad Movement tiene StoreId o Bodega.StoreId
            query = query.Where(m => m.Warehouse.StoreId == request.StoreId);
        }

        // Accedemos a las propiedades privadas mediante reflexión o cambiándolas a 'protected' 
        // en el Request. Aquí asumo que puedes acceder a ellas:
        if (request.MinDate.HasValue)
            query = query.Where(m => m.CreatedAt >= request.MinDate.Value);

        if (request.MaxDate.HasValue)
            query = query.Where(m => m.CreatedAt <= request.MaxDate.Value);

        // 3. Ordenamiento dinámico
        // Si no usas System.Linq.Dynamic.Core, aplicamos el orden por defecto
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            // Ejemplo simple: si contiene "desc" aplicamos OrderByDescending
            query = request.OrderBy.Contains("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(m => m.Id) // O la propiedad permitida
                : query.OrderBy(m => m.Id);
        }

        // 4. Proyección al DTO (Record)
        var projectedQuery = query.Select(m => new MovimientoDetailDto(
            m.Id,
            m.LineaProducto.Id,
            m.LineaProductoId,
            m.WarehouseId,
            m.Type,
            m.Quantity,
            m.UnitCost,
            m.TotalCost,
            m.Lote,
            m.ReferenceTye,
            m.Motive,
            m.Observations,
            m.CreatedAt,
            m.CreatedBy
        ));

        // 5. Paginación (Lógica basada en BaseListRequest)
        // Si PageSize es -1, traemos todo según tus comentarios en el Request
        if (request.PageSize != -1)
        {
            projectedQuery = projectedQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        return await projectedQuery.ToListAsync();
    }
}