using ERP.DATA.Services.VentasService.ClientService;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Ventas.Clientes;

public partial class ClientesDashboard
{
    [Inject] private ClientService ClientService { get; set; } = null!;

    private bool _loading = true;
    private string? _error;
    private string? _search;
    private List<ClientSummaryDto> _items = new();

    // Paginación
    private int _pageNumber = 1;
    private int _pageSize = 20;
    private int _totalCount = 0;
    private int _totalPages => _pageSize > 0 ? (int)Math.Ceiling(_totalCount / (double)_pageSize) : 1;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _loading = true;
        _error = null;
        try
        {
            var request = new ListClientsRequest
            {
                Search = _search,
                PageNumber = _pageNumber,
                PageSize = _pageSize
            };
            var paged = await ClientService.ListPagedAsync(request, CancellationToken.None);
            _items = paged.ToList();
            _totalCount = paged.TotalCount;
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task FilterClients()
    {
        _pageNumber = 1;
        await LoadData();
    }

    private async Task GoToPage(int page)
    {
        if (page < 1 || page > _totalPages) return;
        _pageNumber = page;
        await LoadData();
    }

    private async Task ChangePageSize(int size)
    {
        _pageSize = size;
        _pageNumber = 1;
        await LoadData();
    }
}
