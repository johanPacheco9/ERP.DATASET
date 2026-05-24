using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Payments;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

public partial class SaleService
{
    public async Task<SaleDetailDto> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        var client = await context.Clients.FindAsync([request.ClientId], cancellationToken)
            ?? throw new InvalidOperationException("Cliente no encontrado.");

        var warehouse = await context.Warehouse.FindAsync([request.WarehouseId], cancellationToken)
            ?? throw new InvalidOperationException("Bodega no encontrada.");

        _ = await context.Store.FindAsync([request.StoreId], cancellationToken)
            ?? throw new InvalidOperationException("Tienda no encontrada.");

        using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var sale = new Sale
            {
                SaleNumber = "PENDIENTE",
                ClientId = request.ClientId,
                WarehouseId = request.WarehouseId,
                StoreId = request.StoreId,
                Status = SaleStatus.Completed,
                Notes = request.Notes,
                CreatedBy = request._CreatorAuth0Id,
                CreatedAt = DateTime.UtcNow
            };

            context.Sales.Add(sale);
            await context.SaveChangesAsync(cancellationToken);

            sale.SaleNumber = $"VTA-{DateTime.UtcNow:yyyyMMdd}-{sale.Id:D4}";
            decimal subtotal = 0;
            var lineDetails = new List<SaleLineDetailDto>();

            foreach (var line in request.Lines)
            {
                var linea = await context.LineaProductos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Id == line.LineaProductoId, cancellationToken)
                    ?? throw new InvalidOperationException($"Producto línea {line.LineaProductoId} no existe.");

                var qty = line.ProductoId.HasValue ? 1 : line.Quantity;
                var unitPrice = line.UnitPrice ?? linea.PrecioVenta;
                var lineTotal = unitPrice * qty;

                var movementId = await StockHelper.DeductInventoryAsync(
                    context,
                    request.WarehouseId,
                    line.LineaProductoId,
                    line.ProductoId,
                    qty,
                    $"Venta {sale.SaleNumber}",
                    request._CreatorAuth0Id,
                    sale.Id,
                    cancellationToken);

                string? serialOrSku = null;
                if (line.ProductoId.HasValue)
                {
                    serialOrSku = await context.Productos
                        .Where(p => p.Id == line.ProductoId)
                        .Select(p => p.Serial ?? p.SKU)
                        .FirstAsync(cancellationToken);
                }

                var saleLine = new SaleLineItem
                {
                    SaleId = sale.Id,
                    LineaProductoId = line.LineaProductoId,
                    ProductoId = line.ProductoId,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal,
                    MovementId = movementId,
                    CreatedBy = request._CreatorAuth0Id,
                    CreatedAt = DateTime.UtcNow
                };

                context.SaleLineItems.Add(saleLine);
                subtotal += lineTotal;

                lineDetails.Add(new SaleLineDetailDto(
                    0,
                    linea.Name,
                    serialOrSku,
                    qty,
                    unitPrice,
                    lineTotal,
                    movementId));
            }

            sale.Subtotal = subtotal;
            sale.Total = subtotal;
            
            var payment = new SalePayment
            {
                Sale = sale,
                Amount = request.PaymentAmount,
                Method = request.PaymentMethod,
                PaidAt = DateTime.UtcNow,
                CreatedBy = request._CreatorAuth0Id,
                CreatedAt = DateTime.UtcNow
            };
            context.SalePayments.Add(payment);
            sale.PaymentStatus = request.PaymentAmount >= subtotal
                ? PaymentStatus.Paid
                : request.PaymentAmount > 0
                    ? PaymentStatus.Partial
                    : PaymentStatus.Pending;

            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
           
            return new SaleDetailDto(
                sale.Id,
                sale.SaleNumber,
                sale.CreatedAt,
                client.Name,
                client.IdentificationNumber,
                warehouse.Name,
                sale.Subtotal,
                sale.Total,
                sale.Status.GetDisplayName(),
                sale.PaymentStatus,
                sale.Notes,
                lineDetails);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
