using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

public partial class SaleService
{
    public async Task<List<SaleSummaryDto>> ListAsync(int take = 100, CancellationToken cancellationToken = default)
    {
        var sales = await context.Sales
            .AsNoTracking()
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines)
            .OrderByDescending(s => s.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        return sales.Select(s => new SaleSummaryDto(
            s.Id,
            s.SaleNumber,
            s.CreatedAt,
            s.Client.Name,
            s.Warehouse.Name,
            s.Total,
            s.Status.GetDisplayName(),
            s.PaymentStatus.GetDisplayName(),
            s.Lines.Count)).ToList();
    }

    public async Task<SaleDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await context.Sales
            .AsNoTracking()
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines)
                .ThenInclude(l => l.LineaProducto)
            .Include(s => s.Lines)
                .ThenInclude(l => l.Producto)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sale == null)
            return null;

        var lines = sale.Lines.Select(l => new SaleLineDetailDto(
            l.Id,
            l.LineaProducto.Name,
            l.Producto != null ? (l.Producto.Serial ?? l.Producto.SKU) : null,
            l.Quantity,
            l.UnitPrice,
            l.LineTotal,
            l.MovementId)).ToList();

        return new SaleDetailDto(
            sale.Id,
            sale.SaleNumber,
            sale.CreatedAt,
            sale.Client.Name,
            sale.Client.IdentificationNumber,
            sale.Warehouse.Name,
            sale.Subtotal,
            sale.Total,
            sale.Status.GetDisplayName(),
            sale.PaymentStatus,
            sale.Notes,
            lines);
    }

    public Task<int> GetAvailableStockAsync(int lineaProductoId, int warehouseId, CancellationToken cancellationToken = default)
        => StockHelper.GetAvailableCountAsync(context, lineaProductoId, warehouseId, cancellationToken);
}
