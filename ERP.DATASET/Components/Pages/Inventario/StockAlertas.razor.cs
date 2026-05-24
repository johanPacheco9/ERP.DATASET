using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario;

public partial class StockAlertas
{
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;

    private bool _loading = true;
    private List<StockAlertDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        _items = await WarehouseService.ListStockAlerts(CancellationToken.None);
        _loading = false;
    }
}
