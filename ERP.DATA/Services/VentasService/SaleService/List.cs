using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

public partial class SaleService
{
    public Task<List<SaleSummaryDto>> ListAsync(int take = 100, CancellationToken cancellationToken = default)
        => ListAsync(null, null, take, cancellationToken);

    public async Task<List<SaleSummaryDto>> ListAsync(
        string? search,
        int? warehouseId = null,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        var query = context.Sales
            .AsNoTracking()
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines)
            .AsQueryable();

        if (warehouseId.HasValue && warehouseId.Value > 0)
        {
            query = query.Where(s => s.WarehouseId == warehouseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(v =>
                v.SaleNumber.ToLower().Contains(s) ||
                v.Client.Name.ToLower().Contains(s) ||
                v.Client.IdentificationNumber.ToLower().Contains(s) ||
                (v.FactusInvoiceNumber != null && v.FactusInvoiceNumber.ToLower().Contains(s)));
        }

        var sales = await query
            .OrderByDescending(s => s.CreatedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        return sales.Select(s => new SaleSummaryDto(
            s.Id,
            s.SaleNumber,
            s.CreatedAt,
            s.Client.Name,
            s.Warehouse.Name,
            s.Subtotal,
            s.TaxAmount,
            s.Total,
            s.Status.GetDisplayName(),
            s.PaymentStatus.GetDisplayName(),
            s.Lines.Count,
            s.FactusStatus,
            s.FactusInvoiceNumber)).ToList();
    }

    public async Task<SaleDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await context.Sales
            .AsNoTracking()
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines)
                .ThenInclude(l => l.ProductoVariante)
                    .ThenInclude(v => v.ProductoBase)
            .Include(s => s.Lines)
                .ThenInclude(l => l.UnidadProducto)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (sale == null)
            return null;

        var lines = sale.Lines.Select(l => new SaleLineDetailDto(
            l.Id,
            l.ProductoVarianteId,
            l.ProductoVariante.ProductoBase.Name,
            l.ProductoVariante.SKU,
            l.UnidadProducto?.SerialNumber,
            l.Quantity,
            l.UnitPrice,
            l.TaxRate,
            l.TaxAmount,
            l.LineTotal,
            l.MovementId
        )).ToList();

        return new SaleDetailDto(
            sale.Id,
            sale.SaleNumber,
            sale.CreatedAt,
            sale.Client.Name,
            sale.Client.IdentificationNumber,
            sale.Client.Email,
            sale.Client.PhoneNumber,
            sale.Client.Address,
            sale.Warehouse.Name,
            sale.Subtotal,
            sale.TaxAmount,
            sale.Total,
            sale.Status.GetDisplayName(),
            sale.PaymentStatus,
            sale.Notes,
            sale.FactusInvoiceNumber,
            sale.FactusStatus,
            sale.FactusCufe,
            sale.FactusQrUrl,
            lines);
    }

    public Task<int> GetAvailableStockAsync(int productoVarianteId, int warehouseId, CancellationToken cancellationToken = default)
        => StockHelper.GetAvailableCountAsync(context, productoVarianteId, warehouseId, cancellationToken);
}