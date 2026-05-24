using ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.Payments;

public partial class PaymentsService
{
    public async Task<List<ReceivableSummaryDto>> ListReceivables(bool onlyOpen = true, CancellationToken cancellationToken = default)
    {
        var query = context.Sales
            .AsNoTracking()
            .Include(s => s.Client)
            .Include(s => s.Payments)
            .AsQueryable();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Take(300)
            .Select(s => new
            {
                s.Id,
                s.SaleNumber,
                s.CreatedAt,
                ClientName = s.Client.Name,
                ClientIdentification = s.Client.IdentificationNumber,
                s.Total,
                TotalPaid = s.Payments.Sum(p => p.Amount),
                s.PaymentStatus
            })
            .ToListAsync(cancellationToken);

        return items
            .Select(s => new ReceivableSummaryDto(
                s.Id,
                s.SaleNumber,
                s.CreatedAt,
                s.ClientName,
                s.ClientIdentification,
                s.Total,
                s.TotalPaid,
                s.Total - s.TotalPaid,
                s.PaymentStatus.GetDisplayName()))
            .Where(s => !onlyOpen || s.Balance > 0)
            .OrderByDescending(s => s.Balance)
            .ThenBy(s => s.CreatedAt)
            .ToList();
    }
}
