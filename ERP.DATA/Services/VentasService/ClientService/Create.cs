using ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService
{
    public async Task<ClientSummaryDto?> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.ParametersAreValid(out _))
            return null;

        var exists = await context.Clients.AnyAsync(
            c => c.IdentificationNumber == request.IdentificationNumber,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Ya existe un cliente con esa identificación.");

        var client = new Client
        {
            Name = request.Name.Trim(),
            DniType = request.DniType,
            IdentificationNumber = request.IdentificationNumber.Trim(),
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            City = request.City
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync(cancellationToken);

        return new ClientSummaryDto(
            client.Id,
            client.Name,
            client.DniType,
            client.IdentificationNumber,
            client.PhoneNumber,
            client.City);
    }
}
