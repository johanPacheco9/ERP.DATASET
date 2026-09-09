using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService
{
    public async Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("El Id del cliente no es válido.");

        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new InvalidOperationException($"Cliente {id} no encontrado.");

        if (!client.IsActive)
            return true;

        client.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ReactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("El Id del cliente no es válido.");

        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new InvalidOperationException($"Cliente {id} no encontrado.");

        if (client.IsActive)
            return true;

        client.IsActive = true;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}