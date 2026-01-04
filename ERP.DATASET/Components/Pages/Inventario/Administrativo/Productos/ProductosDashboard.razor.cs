using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos;

public partial class ProductosDashboard
{
    private string _searchText = string.Empty;
    private string _selectedCategory = string.Empty;
    private string _stockFilter = string.Empty;

    private int _currentPage = 1;
    private const int _pageSize = 15;

    private int _totalItems;
    private int _totalPages;

    private bool _nuevoProductoModal = false;
    private bool _isLoading = false;

    private List<ProductoSummaryDto> productos = new();

    [Inject]
    public IProductoService ProductoService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await GetProductos();
    }

    private async Task GetProductos()
    {
        _isLoading = true;

        var request = new ListProductRequest(
            pageNumber: _currentPage,
            pageSize: _pageSize,
            minDate: null,
            maxDate: null,
            orderBy: null
        );

        if (!string.IsNullOrWhiteSpace(_searchText))
        {
            request.OrderBy = _searchText;
        }

        var result = await ProductoService.ListAsync(
            request,
            CancellationToken.None
        );

        productos = result.ToList();
        _totalItems = result.TotalCount;
        _totalPages = result.TotalPages;

        _isLoading = false;
    }
    private async Task ChangePage(int page)
    {
        if (page < 1 || page > _totalPages)
            return;

        _currentPage = page;
        await GetProductos();
    }

    private int TotalPages => _totalPages;

    private async Task Buscar()
    {
        _currentPage = 1;
        await GetProductos();
    }

    private async Task ClearFilters()
    {
        _searchText = string.Empty;
        _selectedCategory = string.Empty;
        _stockFilter = string.Empty;
        _currentPage = 1;

        await GetProductos();
    }

    private void Ver(ProductoSummaryDto p)
    {
        Console.WriteLine($"Viendo producto: {p.Nombre}");
    }

    private void Editar(ProductoSummaryDto p)
    {
        Console.WriteLine($"Editando producto: {p.Nombre}");
    }

    private void Eliminar(ProductoSummaryDto p)
    {
        Console.WriteLine($"Eliminando producto: {p.Nombre}");
    }
    private void NuevoProducto()
    {
        _nuevoProductoModal = true;
    }

    private async Task CerrarModal()
    {
        _nuevoProductoModal = false;
        await GetProductos();
    }
}
