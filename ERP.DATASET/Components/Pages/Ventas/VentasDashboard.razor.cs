using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Ventas;

public partial class VentasDashboard
{
    [Inject] private SaleService SaleService { get; set; } = null!;

    private bool _loading = true;
    private string? _error;
    private List<SaleSummaryDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _items = await SaleService.ListAsync(100, CancellationToken.None);
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
