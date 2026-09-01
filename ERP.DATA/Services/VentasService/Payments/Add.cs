using ERP.TRAN.CrossLayers.API.Pos.Payments.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Payments;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.Payments;

public partial class PaymentsService
{
    public async Task<SalePaymentsSummaryDto> AddPayment(AddPaymentRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        var sale = await context.Sales
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken)
            ?? throw new InvalidOperationException($"Venta {request.SaleId} no encontrada.");

        var totalPaidBefore = sale.Payments.Sum(p => p.Amount);
        var balance = sale.Total - totalPaidBefore;
        if (balance <= 0)
            throw new InvalidOperationException("La venta ya se encuentra pagada.");

        var amount = Math.Min(request.Amount, balance);
        var payment = new SalePayment
        {
            SaleId = sale.Id,
            Amount = amount,
            Method = request.Method,
            PaidAt = request.PaidAt ?? DateTime.UtcNow,
            Reference = request.Reference,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request._CreatorId
        };

        context.SalePayments.Add(payment);

        var totalPaid = totalPaidBefore + amount;
        sale.PaymentStatus = totalPaid >= sale.Total
            ? PaymentStatus.Paid
            : totalPaid > 0
                ? PaymentStatus.Partial
                : PaymentStatus.Pending;
        sale.UpdatedAt = DateTime.UtcNow;
        sale.UpdatedBy = request._CreatorId;

        await context.SaveChangesAsync(cancellationToken);

        return await GetBySaleId(sale.Id, cancellationToken);
    }
}
