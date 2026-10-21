using ERP.TRAN.CrossLayers.API.Base.ResultPattern;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.SaleService;

public partial class SaleService
{
    /// <summary>
    /// Anula una venta completa: revierte el inventario de todas las líneas,
    /// marca la venta como Cancelled y registra el historial (SaleReturn) por línea.
    /// </summary>
    public async Task<Result<SaleDetailDto>> CancelSaleAsync(
        int saleId,
        string motivo,
        int createdBy,
        CancellationToken cancellationToken = default)
    {
        if (saleId <= 0)
            return Result<SaleDetailDto>.Failure(new Error("Sale.InvalidId", "El id de la venta es obligatorio."));

        if (string.IsNullOrWhiteSpace(motivo))
            return Result<SaleDetailDto>.Failure(new Error("Sale.InvalidReason", "El motivo de anulación es obligatorio."));

        var sale = await context.Sales
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines)
                .ThenInclude(l => l.ProductoVariante).ThenInclude(v => v.ProductoBase)
            .Include(s => s.Lines)
                .ThenInclude(l => l.UnidadProducto)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken);

        if (sale is null)
            return Result<SaleDetailDto>.Failure(new Error("Sale.NotFound", "Venta no encontrada."));

        if (sale.Status == SaleStatus.Cancelled)
            return Result<SaleDetailDto>.Failure(new Error("Sale.AlreadyCancelled", "La venta ya está anulada."));

        return await Result<SaleDetailDto>.TryAsync(async () =>
        {
            await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

            foreach (var line in sale.Lines)
            {
                await StockHelper.ReturnInventoryAsync(
                    context,
                    sale.WarehouseId,
                    line.ProductoVarianteId,
                    line.UnidadProductoId,
                    line.Quantity,
                    $"Anulación venta #{sale.Id}: {motivo}",
                    createdBy,
                    sale.Id,
                    cancellationToken);

                context.SaleReturns.Add(new SaleReturn
                {
                    SaleId = sale.Id,
                    SaleLineItemId = line.Id,
                    ClientId = sale.ClientId,
                    ProductoVarianteId = line.ProductoVarianteId,
                    UnidadProductoId = line.UnidadProductoId,
                    Quantity = line.Quantity,
                    Motivo = motivo,
                    IsFullCancellation = true,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                });
            }

            sale.Status = SaleStatus.Cancelled;
            sale.Notes = string.IsNullOrWhiteSpace(sale.Notes)
                ? $"Anulada: {motivo}"
                : $"{sale.Notes}\nAnulada: {motivo}";

            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            var dto = await GetByIdAsync(sale.Id, cancellationToken);
            return dto ?? throw new InvalidOperationException("No se pudo recargar la venta.");
        },
        ex => ex is DbUpdateConcurrencyException
            ? new Error("Sale.ConcurrencyConflict", "Conflicto de concurrencia al anular la venta. Reintente.")
            : new Error("Sale.UnexpectedError", ex.Message));
    }

    /// <summary>
    /// Devuelve parcialmente una línea de venta: revierte el inventario indicado,
    /// mantiene la venta activa y registra el historial (SaleReturn).
    /// </summary>
    public async Task<Result<SaleDetailDto>> ReturnLineAsync(
        int saleId,
        int lineId,
        int quantity,
        string? serialNumber,
        string motivo,
        int createdBy,
        CancellationToken cancellationToken = default)
    {
        if (saleId <= 0 || lineId <= 0)
            return Result<SaleDetailDto>.Failure(new Error("Sale.InvalidId", "Ids inválidos."));

        if (quantity <= 0)
            return Result<SaleDetailDto>.Failure(new Error("Sale.InvalidQuantity", "La cantidad a devolver debe ser mayor a cero."));

        if (string.IsNullOrWhiteSpace(motivo))
            return Result<SaleDetailDto>.Failure(new Error("Sale.InvalidReason", "El motivo de devolución es obligatorio."));

        var sale = await context.Sales
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines)
                .ThenInclude(l => l.ProductoVariante).ThenInclude(v => v.ProductoBase)
            .Include(s => s.Lines)
                .ThenInclude(l => l.UnidadProducto)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken);

        if (sale is null)
            return Result<SaleDetailDto>.Failure(new Error("Sale.NotFound", "Venta no encontrada."));

        if (sale.Status == SaleStatus.Cancelled)
            return Result<SaleDetailDto>.Failure(new Error("Sale.Cancelled", "No se puede devolver sobre una venta anulada."));

        var line = sale.Lines.FirstOrDefault(l => l.Id == lineId);
        if (line is null)
            return Result<SaleDetailDto>.Failure(new Error("Sale.LineNotFound", "Línea de venta no encontrada."));

        // Cantidad ya devuelta para esta línea.
        var yaDevuelto = await context.SaleReturns
            .Where(r => r.SaleLineItemId == lineId)
            .SumAsync(r => (int?)r.Quantity, cancellationToken) ?? 0;

        if (quantity + yaDevuelto > line.Quantity)
            return Result<SaleDetailDto>.Failure(new Error("Sale.ReturnExceeds",
                $"Cantidad a devolver ({quantity}) supera lo disponible (pendiente {line.Quantity - yaDevuelto})."));

        // Resolver serial/unidad.
        int? unidadId = line.UnidadProductoId;
        if (line.ProductoVariante.ProductoBase.RequiereSerial)
        {
            if (string.IsNullOrWhiteSpace(serialNumber) && line.UnidadProducto is null)
                return Result<SaleDetailDto>.Failure(new Error("Sale.SerialRequired", "El serial es obligatorio para devolver este producto."));

            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                var unidad = await context.UnidadesProductos
                    .FirstOrDefaultAsync(u => u.SerialNumber == serialNumber.Trim() &&
                                             u.ProductoVarianteId == line.ProductoVarianteId &&
                                             u.BodegaId == sale.WarehouseId, cancellationToken);
                if (unidad is null)
                    return Result<SaleDetailDto>.Failure(new Error("Sale.UnitNotFound", "Unidad física no encontrada en la bodega."));
                unidadId = unidad.Id;
            }

            if (quantity != 1)
                return Result<SaleDetailDto>.Failure(new Error("Sale.InvalidQuantity", "Para productos serializados la cantidad debe ser 1."));
        }

        return await Result<SaleDetailDto>.TryAsync(async () =>
        {
            await using var tx = await context.Database.BeginTransactionAsync(cancellationToken);

            await StockHelper.ReturnInventoryAsync(
                context,
                sale.WarehouseId,
                line.ProductoVarianteId,
                unidadId,
                quantity,
                $"Devolución venta #{sale.Id} línea #{line.Id}: {motivo}",
                createdBy,
                sale.Id,
                cancellationToken);

            context.SaleReturns.Add(new SaleReturn
            {
                SaleId = sale.Id,
                SaleLineItemId = line.Id,
                ClientId = sale.ClientId,
                ProductoVarianteId = line.ProductoVarianteId,
                UnidadProductoId = unidadId,
                Quantity = quantity,
                Motivo = motivo,
                IsFullCancellation = false,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            var dto = await GetByIdAsync(sale.Id, cancellationToken);
            return dto ?? throw new InvalidOperationException("No se pudo recargar la venta.");
        },
        ex => ex is DbUpdateConcurrencyException
            ? new Error("Sale.ConcurrencyConflict", "Conflicto de concurrencia al devolver. Reintente.")
            : new Error("Sale.UnexpectedError", ex.Message));
    }

    /// <summary>Lista el historial de anulaciones/devoluciones de una venta.</summary>
    public async Task<List<SaleReturn>> ListReturnsAsync(int saleId, CancellationToken cancellationToken = default)
    {
        return await context.SaleReturns
            .AsNoTracking()
            .Where(r => r.SaleId == saleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
