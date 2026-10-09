using ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Services.VentasService.ClientService;

public partial class ClientService
{
    /// <summary>
    /// Lista clientes con paginación real, ordenados por nombre.
    /// </summary>
    public async Task<PagedList<ClientSummaryDto>> ListPagedAsync(
        ListClientsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!request.ParametersAreValid(out var errors))
            throw new InvalidOperationException(errors);

        var query = context.Clients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(s) ||
                c.IdentificationNumber.ToLower().Contains(s) ||
                (c.Email != null && c.Email.ToLower().Contains(s)) ||
                (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(s)));
        }

        var projection = query
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .Select(c => new ClientSummaryDto(
                c.Id,
                c.Name,
                c.DniType,
                c.IdentificationNumber,
                c.PhoneNumber,
                c.City,
                c.Email,
                c.Address,
                c.Dv));

        return await PagedList<ClientSummaryDto>.ToPagedListAsync(
            projection,
            request.PageNumber,
            request.PageSize);
    }
}