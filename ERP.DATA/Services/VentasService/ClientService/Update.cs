using ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService
{
    public async Task<ClientSummaryDto> UpdateAsync(UpdateClientRequest request, CancellationToken cancellationToken = default)
    {
        if (!request.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Cliente {request.Id} no encontrado.");

        if (client.IdentificationNumber != request.IdentificationNumber.Trim())
        {
            var exists = await context.Clients.AnyAsync(
                c => c.IdentificationNumber == request.IdentificationNumber && c.Id != request.Id,
                cancellationToken);

            if (exists)
                throw new InvalidOperationException("Ya existe otro cliente con esa identificación.");
        }

        client.Name = request.Name.Trim();
        client.DniType = request.DniType;
        client.IdentificationNumber = request.IdentificationNumber.Trim();
        client.Dv = request.Dv;
        client.Email = request.Email?.Trim();
        client.PhoneNumber = request.PhoneNumber?.Trim();
        client.Address = request.Address?.Trim();
        client.City = request.City?.Trim();
        client.TaxRegime = request.TaxRegime?.Trim();

        await context.SaveChangesAsync(cancellationToken);

        return new ClientSummaryDto(
            client.Id,
            client.Name,
            client.DniType,
            client.IdentificationNumber,
            client.PhoneNumber,
            client.City,
            client.Email,
            client.Address,
            client.Dv,
            client.IsActive);
    }
}