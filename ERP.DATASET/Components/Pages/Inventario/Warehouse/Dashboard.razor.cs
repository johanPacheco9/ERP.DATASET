using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ERP.DATASET.Components.Pages.Inventario.Warehouse;

public partial class Dashboard : ComponentBase
{
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private List<WarehouseSummaryDto> _bodegas = new();
    private bool _loading = true;
    private string? _error;
    private string _search = "";
    
    // Paginación
    private int _page = 1;
    private const int PageSize = 10;
    private int _total = 0;
    private int _totalPages = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadWarehouses();
    }

    private async Task LoadWarehouses()
    {
        _loading = true;
        _error = null;
        try
        {
            var request = new ListWarehousesRequest
            {
                PageNumber = _page,
                PageSize = PageSize,
                SearchTerm = string.IsNullOrWhiteSpace(_search) ? null : _search.Trim()
            };

            var result = await WarehouseService.List(request, CancellationToken.None);
            
            // Asumiendo que tu método retorna un PagedList o colección paginable con TotalCount
            _bodegas = result.ToList();
            _total = result.TotalCount;
            _totalPages = result.TotalPages;
        }
        catch (Exception e)
        {
            _error = $"No se pudieron cargar las bodegas: {e.Message}";
            _bodegas = new();
            Console.WriteLine(e);
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task Buscar()
    {
        _page = 1;
        await LoadWarehouses();
    }

    private async Task LimpiarFiltro()
    {
        _search = "";
        _page = 1;
        await LoadWarehouses();
    }

    private async Task CambiarPagina(int nuevaPagina)
    {
        if (nuevaPagina < 1 || nuevaPagina > _totalPages) return;
        _page = nuevaPagina;
        await LoadWarehouses();
    }

    private async Task OnSearchKey(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Buscar();
        }
    }

    private void IrANuevaBodega() => NavigationManager.NavigateTo("/bodegas/nuevo");
    private void IrAEditar(int id) => NavigationManager.NavigateTo($"/bodegas/editar/{id}");
}