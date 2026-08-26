using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.InventarioService.WarehouseService;

public partial class WarehouseService
{
    public async Task<List<StockAlertDto>> ListStockAlerts(CancellationToken cancellationToken = default)
    {
        var items = await context.WarehouseStock // Asegúrate de que el DbSet sea WarehouseStocks o WarehouseStock según tu DbContext
            .AsNoTracking()
            .Include(s => s.Warehouse)
            .Include(s => s.ProductoVariante)
            .ThenInclude(v => v.ProductoBase)
            .Where(s => s.StockMinimo > 0 && s.CurrentStock <= s.StockMinimo) // Ajustado a 'Quantity' (o CurrentStock si tu propiedad se llama así)
            .OrderBy(s => s.CurrentStock - s.StockMinimo)
            .ThenBy(s => s.ProductoVariante.ProductoBase.Name)
            .Take(300)
            .Select(s => new StockAlertDto(
                s.WarehouseId,
                s.Warehouse.Name,
                s.ProductoVarianteId,
                s.ProductoVariante.ProductoBaseId,
                s.ProductoVariante.ProductoBase.Code,
                s.ProductoVariante.ProductoBase.Name,
                s.CurrentStock,          // Si en tu WarehouseStock la propiedad de stock actual se llama 'Quantity'
                s.StockReservado,
                s.StockMinimo,
                s.StockMaximo,
                s.StockMaximo > 0
                    ? Math.Max(0, s.StockMaximo - s.CurrentStock)
                    : Math.Max(0, s.StockMinimo - s.CurrentStock)
            ))
            .ToListAsync(cancellationToken);

        return items;
    }
}