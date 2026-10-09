using ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService
{
    public async Task<ClientSummaryDto> UpdateAsync(int id, UpdateClientRequest request, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new InvalidOperationException("El cliente es obligatorio.");

        if (!request.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new InvalidOperationException($"Cliente {id} no encontrado.");

        var identification = request.IdentificationNumber.Trim();

        // Si cambia la identificación, no puede chocar con la de otro cliente.
        if (client.IdentificationNumber != identification)
        {
            var exists = await context.Clients.AnyAsync(
                c => c.IdentificationNumber == identification && c.Id != id,
                cancellationToken);

            if (exists)
                throw new InvalidOperationException("Ya existe otro cliente con esa identificación.");
        }

        client.Name = request.Name.Trim();
        client.DniType = request.DniType;
        client.IdentificationNumber = identification;
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
            client.Dv);
    }
}