using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Ventas;

public partial class VentasDashboard
{
    [Inject] private SaleService SaleService { get; set; } = null!;
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;

    private bool _loading = true;
    private string? _error;
    private List<SaleSummaryDto> _items = new();
    private List<WarehouseSummaryDto> _bodegas = new();

    private string? _search;
    private int _selectedWarehouseId = 0;

    private decimal TotalFacturado => _items.Sum(v => v.Total);
    private decimal TotalIva => _items.Sum(v => v.TaxAmount);

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
            var bodegas = await WarehouseService.List(new ListWarehousesRequest { PageNumber = 1, PageSize = 50 }, CancellationToken.None);
            _bodegas = bodegas.ToList();

            await FilterSales();
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

    private async Task FilterSales()
    {
        _error = null;
        try
        {
            var wh = _selectedWarehouseId > 0 ? _selectedWarehouseId : (int?)null;
            _items = await SaleService.ListAsync(_search, wh, 100, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
    }
}
