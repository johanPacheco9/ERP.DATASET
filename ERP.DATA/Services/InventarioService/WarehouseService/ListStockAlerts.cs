using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.WarehouseService;

public partial class WarehouseService
{
    public async Task<List<StockAlertDto>> ListStockAlerts(CancellationToken cancellationToken = default)
    {
        var items = await context.WarehouseStock
            .AsNoTracking()
            .Include(s => s.Warehouse)
            .Include(s => s.LineaProducto)
            .Where(s => s.StockMinimo > 0 && s.CurrentStock <= s.StockMinimo)
            .OrderBy(s => s.CurrentStock - s.StockMinimo)
            .ThenBy(s => s.LineaProducto.Name)
            .Take(300)
            .Select(s => new StockAlertDto(
                s.WarehouseId,
                s.Warehouse.Name,
                s.LineaProductoId,
                s.LineaProducto.Code,
                s.LineaProducto.Name,
                s.CurrentStock,
                s.StockReservado,
                s.StockMinimo,
                s.StockMaximo,
                s.StockMaximo > 0
                    ? Math.Max(0, s.StockMaximo - s.CurrentStock)
                    : Math.Max(0, s.StockMinimo - s.CurrentStock)))
            .ToListAsync(cancellationToken);

        return items;
    }
}
