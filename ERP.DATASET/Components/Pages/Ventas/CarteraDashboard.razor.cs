using ERP.DATA.Services.VentasService.Payments;
using ERP.TRAN.CrossLayers.API.Pos.Payments.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Ventas;

public partial class CarteraDashboard
{
    [Inject] private PaymentsService PaymentsService { get; set; } = null!;

    private bool _loading = true;
    private bool _onlyOpen = true;
    private string _search = string.Empty;
    private List<ReceivableSummaryDto> _items = new();

    private decimal TotalBalance => _items.Sum(i => Math.Max(0, i.Balance));
    private decimal TotalPaid => _items.Sum(i => i.TotalPaid);

    private IEnumerable<ReceivableSummaryDto> FilteredItems =>
        string.IsNullOrWhiteSpace(_search)
            ? _items
            : _items.Where(i =>
                i.SaleNumber.Contains(_search, StringComparison.OrdinalIgnoreCase) ||
                i.ClientName.Contains(_search, StringComparison.OrdinalIgnoreCase) ||
                i.ClientIdentification.Contains(_search, StringComparison.OrdinalIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _loading = true;
        _items = await PaymentsService.ListReceivables(_onlyOpen, CancellationToken.None);
        _loading = false;
    }
}
