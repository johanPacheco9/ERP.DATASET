using ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.DATA.Services.VentasService.Payments;

public partial class PaymentsService
{
    public async Task<SalePaymentsSummaryDto> GetBySaleId(int saleId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (saleId <= 0)
                throw new ArgumentException("El Id de la venta no es válido.");

            var sale = await context.Sales
                           .AsNoTracking()
                           .Include(s => s.Payments)
                           .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken)
                       ?? throw new InvalidOperationException($"Venta {saleId} no encontrada.");

            var payments = sale.Payments.Select(p => new GetPaymentsBySaleResponseDto(
                p.Id,
                p.Amount,
                p.Method.GetDisplayName(),
                p.PaidAt,
                p.Reference,
                p.Notes,
                p.CreatedBy
            )).ToList();

            var totalPaid = payments.Sum(p => p.Amount);

            return new SalePaymentsSummaryDto(
                sale.Id,
                sale.Total,
                totalPaid,
                sale.Total - totalPaid,
                sale.PaymentStatus.GetDisplayName(),
                payments
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error obteniendo pagos de la venta {SaleId}", saleId);
            throw;
        }
    }
}