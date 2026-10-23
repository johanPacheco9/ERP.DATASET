using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;
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

    // Paginación
    private int _pageNumber = 1;
    private int _pageSize = 20;
    private int _totalCount = 0;
    private int _totalPages => _pageSize > 0 ? (int)Math.Ceiling(_totalCount / (double)_pageSize) : 1;

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

    private async Task LoadPage()
    {
        _error = null;
        try
        {
            var wh = _selectedWarehouseId > 0 ? _selectedWarehouseId : (int?)null;
            var request = new ListSalesRequest
            {
                Search = _search,
                WarehouseId = wh,
                PageNumber = _pageNumber,
                PageSize = _pageSize
            };
            var paged = await SaleService.ListPagedAsync(request, CancellationToken.None);
            _items = paged.ToList();
            _totalCount = paged.TotalCount;
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
    }

    private async Task FilterSales()
    {
        _pageNumber = 1;
        await LoadPage();
    }

    private async Task GoToPage(int page)
    {
        if (page < 1 || page > _totalPages) return;
        _pageNumber = page;
        await LoadPage();
    }

    private async Task ChangePageSize(int size)
    {
        _pageSize = size;
        _pageNumber = 1;
        await LoadPage();
    }
}
