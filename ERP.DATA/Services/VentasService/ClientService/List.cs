using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService
{
    public async Task<List<ClientSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await context.Clients
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ClientSummaryDto(
                c.Id,
                c.Name,
                c.DniType,
                c.IdentificationNumber,
                c.PhoneNumber,
                c.City))
            .ToListAsync(cancellationToken);
    }
}
