using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
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
    public async Task<Result<SaleDetailDto>> CreateAsync(CreateSaleRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.ParametersAreValid(out var errors))
            return Result<SaleDetailDto>.Failure(new Error("Sale.InvalidParameters", errors));

        var client = await context.Clients.FindAsync([request.ClientId], cancellationToken);
        if (client == null)
            return Result<SaleDetailDto>.Failure(new Error("Sale.ClientNotFound", "Cliente no encontrado."));

        var warehouse = await context.Warehouse.FindAsync([request.WarehouseId], cancellationToken);
        if (warehouse == null)
            return Result<SaleDetailDto>.Failure(new Error("Sale.WarehouseNotFound", "Bodega no encontrada."));

        var store = await context.Store.FindAsync([request.StoreId], cancellationToken);
        if (store == null)
            return Result<SaleDetailDto>.Failure(new Error("Sale.StoreNotFound", "Tienda no encontrada."));

        return await Result<SaleDetailDto>.TryAsync(async () =>
        {
            await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

            var sale = new Sale
            {
                SaleNumber = "PENDIENTE",
                ClientId = request.ClientId,
                WarehouseId = request.WarehouseId,
                StoreId = request.StoreId,
                Status = SaleStatus.Completed,
                Notes = request.Notes,
                CreatedBy = request._CreatorAuth0Id,
                CreatedAt = DateTime.UtcNow,
                FactusStatus = "Pendiente"
            };

            context.Sales.Add(sale);
            await context.SaveChangesAsync(cancellationToken);

            sale.SaleNumber = $"POS-{DateTime.UtcNow:yyyyMMdd}-{sale.Id:D4}";
            decimal subtotal = 0;
            decimal totalTax = 0;

            // Lista temporal para almacenar referencias antes de construir el DTO final
            var pendingLineItems = new List<(SaleLineItem Item, string ProductName, string Sku, string? SerialNumber)>();

            foreach (var line in request.Lines)
            {
                // 1. Obtener Variante y su ProductoBase de forma segura por ID de variante
                var variante = await context.ProductoVariantes
                    .AsNoTracking()
                    .Include(v => v.ProductoBase)
                    .FirstOrDefaultAsync(v => v.Id == line.ProductoVarianteId, cancellationToken)
                    ?? throw new InvalidOperationException($"La variante con ID #{line.ProductoVarianteId} no existe.");

                // Validación estricta según RequiereSerial de ProductoBase
                string? serialNumberAUsar;
                int qty;

                if (variante.ProductoBase.RequiereSerial)
                {
                    if (string.IsNullOrWhiteSpace(line.SerialNumber))
                        throw new InvalidOperationException($"El producto '{variante.ProductoBase.Name}' requiere número de serial / IMEI para la venta.");

                    serialNumberAUsar = line.SerialNumber.Trim();
                    qty = 1;
                }
                else
                {
                    if (line.Quantity <= 0)
                        throw new InvalidOperationException($"La cantidad para '{variante.ProductoBase.Name}' debe ser mayor a cero.");

                    serialNumberAUsar = null; // Asegura que los productos sin serial pasen por FIFO real
                    qty = line.Quantity;
                }

                // Fallback de Precio y Tarifa de IVA (Variante -> ProductoBase)
                var unitPrice = line.UnitPrice ?? (variante.PrecioVenta > 0 ? variante.PrecioVenta : variante.ProductoBase.PrecioVenta);
                var taxRate = line.TaxRate ?? (variante.ProductoBase.ExentoIVA ? 0m : variante.ProductoBase.PorcentajeIVA);

                var lineSubtotal = unitPrice * qty;
                var lineTax = Math.Round((lineSubtotal ?? 0) * taxRate, 2);
                var lineTotal = lineSubtotal + lineTax;

                // 2. Descontar Inventario vía StockHelper (usando el ProductoBaseId de la relación real de la variante)
                var (movementId, unidadProductoId, costoVentaTotal) = await StockHelper.DeductInventoryAsync(
                    context,
                    request.WarehouseId,
                    variante.ProductoBaseId,
                    line.ProductoVarianteId,
                    qty,
                    $"Venta {sale.SaleNumber}",
                    request._CreatorAuth0Id,
                    sale.Id,
                    serialNumberAUsar,
                    cancellationToken);

                // 3. Registrar la línea de venta
                var saleLine = new SaleLineItem
                {
                    SaleId = sale.Id,
                    ProductoVarianteId = line.ProductoVarianteId,
                    UnidadProductoId = unidadProductoId,
                    Quantity = qty,
                    UnitPrice = unitPrice ?? 0,
                    TaxRate = taxRate,
                    TaxAmount = lineTax,
                    LineTotal = lineTotal ?? 0,
                    MovementId = movementId,
                    CostoVentaTotal = costoVentaTotal,
                    CreatedBy = request._CreatorAuth0Id,
                    CreatedAt = DateTime.UtcNow
                };

                context.SaleLineItems.Add(saleLine);
                subtotal += lineSubtotal ?? 0;
                totalTax += lineTax;

                pendingLineItems.Add((saleLine, variante.ProductoBase.Name, variante.SKU, line.SerialNumber));
            }

            sale.Subtotal = subtotal;
            sale.TaxAmount = totalTax;
            sale.Total = subtotal + totalTax;

            decimal? cashReceived = null;
            decimal? changeGiven = null;
            if (request.PaymentMethod == PaymentMethod.Cash && request.CashReceived.HasValue)
            {
                cashReceived = request.CashReceived.Value;
                changeGiven = Math.Max(0, cashReceived.Value - request.PaymentAmount);
            }

            var payment = new SalePayment
            {
                Sale = sale,
                Amount = request.PaymentAmount,
                Method = request.PaymentMethod,
                CashReceived = cashReceived,
                ChangeGiven = changeGiven,
                PaidAt = DateTime.UtcNow,
                CreatedBy = request._CreatorAuth0Id,
                CreatedAt = DateTime.UtcNow
            };
            context.SalePayments.Add(payment);

            sale.PaymentStatus = request.PaymentAmount >= sale.Total
                ? PaymentStatus.Paid
                : request.PaymentAmount > 0
                    ? PaymentStatus.Partial
                    : PaymentStatus.Pending;

            // Guardar cambios finales para generar las PKs de SaleLineItems y SalePayment
            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            // Mapear los DTOs de salida con los IDs persistidos
            var lineDetails = pendingLineItems.Select(x => new SaleLineDetailDto(
                x.Item.Id,
                x.Item.ProductoVarianteId,
                x.ProductName,
                x.Sku,
                x.SerialNumber,
                x.Item.Quantity,
                x.Item.UnitPrice,
                x.Item.TaxRate,
                x.Item.TaxAmount,
                x.Item.LineTotal,
                x.Item.MovementId
            )).ToList();

            return new SaleDetailDto(
                sale.Id,
                sale.SaleNumber,
                sale.CreatedAt,
                client.Name,
                client.IdentificationNumber,
                client.Email,
                client.PhoneNumber,
                client.Address,
                warehouse.Name,
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
                lineDetails);
        },
        ex => new Error("Sale.UnexpectedError", ex.Message));
    }
}