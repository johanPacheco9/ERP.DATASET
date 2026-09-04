using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Responses;
using Microsoft.AspNetCore.Components;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos;

public partial class ProductosDashboard
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    private string _searchText = string.Empty;
    private string _selectedCategory = string.Empty;
    private string _stockFilter = string.Empty;

    private int _currentPage = 1;
    private const int PageSize = 15;

    private int _totalItems;
    private int _totalPages;

    private bool _nuevoProductoModal = false;
    private bool _isLoading = false;

    private List<ProductoSummaryDto> _productos = new();

    private List<CategoriaDetailDto> _categorias = [] ;

    [Inject] public ProductoBaseService ProductoService { get; set; } = null!;

    [Inject] public CategoriaService CategoriaService { get; set; } = null!;


    protected override async Task OnInitializedAsync()
    {
        await GetCategorias();
        await GetProductos();
    }


    private async Task GetCategorias()
    {
        var request = new ListCategoriasRequest(
            1, 100, minDate: null, maxDate: null
        );

        var result = await CategoriaService.List(request, CancellationToken.None);
        if (result.Count>0)
        {
            _categorias = result.ToList();
        }
        
    }

    private async Task GetProductos()
    {
        _isLoading = true;

        var request = new ListProductRequest(
            pageNumber: _currentPage,
            pageSize: PageSize,
            minDate: null,
            maxDate: null,
            orderBy: null
        );

        var result = await ProductoService.ListAsync(
            request,
            searchTerm: _searchText,
            categoryName: _selectedCategory,
            stockFilter: _stockFilter,
            CancellationToken.None
        );

        _productos = result.ToList();
        _totalItems = result.TotalCount;
        _totalPages = result.TotalPages;

        _isLoading = false;
    }


    // Controladores de eventos de filtros
    private async Task OnSearchInput(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;
        _currentPage = 1;
        await GetProductos();
    }

    private async Task OnCategoryChanged(ChangeEventArgs e)
    {
        _selectedCategory = e.Value?.ToString() ?? string.Empty;
        _currentPage = 1;
        await GetProductos();
    }

    private async Task OnStockFilterChanged(ChangeEventArgs e)
    {
        _stockFilter = e.Value?.ToString() ?? string.Empty;
        _currentPage = 1;
        await GetProductos();
    }

    private async Task ChangePage(int page)
    {
        if (page < 1 || page > _totalPages || page == _currentPage)
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
        NavigationManager.NavigateTo("/inventario/productos/nuevo");
    }

    private async Task CerrarModal()
    {
        _nuevoProductoModal = false;
        await GetProductos();
    }
}