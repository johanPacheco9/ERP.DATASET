using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService
{
    public Task<List<ClientSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
        => ListAsync(null, cancellationToken);

    public async Task<List<ClientSummaryDto>> ListAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = context.Clients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(s) ||
                c.IdentificationNumber.ToLower().Contains(s) ||
                (c.Email != null && c.Email.ToLower().Contains(s)) ||
                (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(s)));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new ClientSummaryDto(
                c.Id,
                c.Name,
                c.DniType,
                c.IdentificationNumber,
                c.PhoneNumber,
                c.City,
                c.Email,
                c.Address,
                c.Dv))
            .ToListAsync(cancellationToken);
    }
}
