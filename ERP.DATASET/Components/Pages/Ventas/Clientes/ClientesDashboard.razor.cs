using ERP.DATA.Services.VentasService.ClientService;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Ventas.Clientes;

public partial class ClientesDashboard
{
    [Inject] private ClientService ClientService { get; set; } = null!;

    private bool _loading = true;
    private string? _error;
    private List<ClientSummaryDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _items = await ClientService.ListAsync(CancellationToken.None);
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
}
