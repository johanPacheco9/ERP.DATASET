using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Bodegas;

public partial class BodegasDashboard
{
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;

    private bool _loading = true;
    private List<WarehouseSummaryDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        var result = await WarehouseService.List(
            new ListWarehousesRequest { PageNumber = 1, PageSize = 50 },
            CancellationToken.None);
        _items = result.ToList();
        _loading = false;
    }
}
