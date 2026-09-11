using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.DATA.Services.InventarioService.UnidadProductoService;
using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.UserService;
using ERP.DATA.Services.VentasService.Stores;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Stores.Responses;
using ERP.TRAN.CrossLayers.API.Users.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ProductoBaseManager = ERP.DATA.Services.InventarioService.BaseProducto.ProductoBaseManager;

namespace ERP.DATASET.Components.Pages.Inventario.Unidades;

public partial class UnidadesDashboard
{
    [Inject] private UnidadProductoManager UnidadProductoManager { get; set; } = null!;
    [Inject] private ProductoBaseManager ProductoManager { get; set; } = null!;
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private CategoriaService CategoriaService { get; set; } = null!;
    [Inject] private UserManager UserManager { get; set; } = null!;
    [Inject] private StoresManager StoreService { get; set; } = null!; // Ajusta según el servicio que uses para listar tiendas
    
    private UserDetailDto _user;

    private bool _loading = true;
    private string? _error;
    private string _search = "";
    private int _page = 1;
    private const int PageSize = 10;
    private int _total;
    private int _totalPages;
    private int _totalCatalogo;
    private List<UnidadProductoDetailDto> _items = new();

    private List<CategoriaDetailDto> _categorias = new();
    private List<WarehouseSummaryDto> _bodegas = new();
    private List<StoreSummaryDto> _tiendas = new();
    
    private int? _categoriaFiltro;
    private int? _bodegaFiltro;
    private int? _tiendaFiltro;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var catalogo = await ProductoManager.ListAsync(
                request: new ListProductRequest(pageNumber: 1, pageSize: 1),
                searchTerm: null,
                categoryName: null,
                stockFilter: null,
                cancellationToken: CancellationToken.None
            );

            _totalCatalogo = catalogo.TotalCount;
        }
        catch { /* opcional */ }

        await GetUserAutenticate();
        
        // Inicializamos la tienda filtro con la tienda del usuario logueado
        _tiendaFiltro = _user.StoreId;

        await GetTiendasPermitidas();
        await GetBodegasParaTienda(_tiendaFiltro);
        await GetCategorias();
        await Cargar();
    }

    private async Task GetUserAutenticate()
    {
        var user = await UserManager.GetUserAuthenticate();
        if (user.IsSuccess)
        {
            _user = user.Value;
        }
        else
        {
            _error = user.Error?.Message ?? "No se pudo obtener el usuario autenticado.";
        }
    }

    private async Task GetTiendasPermitidas()
    {
        try
        {
            var request = new ListStoresRequest(pageNumber: 1, pageSize: 100); 
            
            var pagedResult = await StoreService.List(request, null, CancellationToken.None);
        
            // Extraemos la lista del PagedList que ya retorna tu método
            _tiendas = pagedResult;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _tiendas = new();
        }
    }

    private async Task GetBodegasParaTienda(int? storeId)
    {
        var request = new ListWarehousesRequest()
        {
            StoreId = storeId
        };

        try
        {
            var bodegas = await WarehouseService.List(request, CancellationToken.None);
            _bodegas = bodegas.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _bodegas = new();
        }
    }

    private async Task GetCategorias()
    {
        var request = new ListCategoriasRequest(pageNumber: 1, pageSize: -1);

        try
        {
            var categorias = await CategoriaService.List(request, CancellationToken.None);
            _categorias = categorias.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task Cargar()
    {
        _loading = true;
        _error = null;
        try
        {
            var request = new ListUnitProductRequest(_page, PageSize)
            {
                Search = string.IsNullOrWhiteSpace(_search) ? null : _search.Trim(),
                CategoryId = _categoriaFiltro,
                BodegaId = _bodegaFiltro,
            };

            var result = await UnidadProductoManager.ListAsync(request, CancellationToken.None);
            _items = result.ToList();
            _total = result.TotalCount;
            _totalPages = result.TotalPages;
        }
        catch (Exception ex)
        {
            _error = $"No se pudieron cargar las unidades: {ex.Message}";
            _items = new();
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task Buscar()
    {
        _page = 1;
        await Cargar();
    }

    private async Task Limpiar()
    {
        _search = "";
        _categoriaFiltro = null;
        _bodegaFiltro = null;
        _tiendaFiltro = _user.StoreId;
        _page = 1;
        await GetBodegasParaTienda(_tiendaFiltro);
        await Cargar();
    }

    private async Task FiltrarPorCategoria(ChangeEventArgs e)
    {
        _categoriaFiltro = int.TryParse(e.Value?.ToString(), out var id) ? id : null;
        _page = 1;
        await Cargar();
    }

    private async Task FiltrarPorBodega(ChangeEventArgs e)
    {
        _bodegaFiltro = int.TryParse(e.Value?.ToString(), out var id) ? id : null;
        _page = 1;
        await Cargar();
    }

    private async Task FiltrarPorTiendas(ChangeEventArgs e)
    {
        _tiendaFiltro = int.TryParse(e.Value?.ToString(), out var id) ? id : null;
        
        // Al cambiar de tienda, reseteamos la bodega seleccionada para evitar incongruencias
        _bodegaFiltro = null;

        await GetBodegasParaTienda(_tiendaFiltro);

        _page = 1;
        await Cargar();
    }

    private async Task CambiarPagina(int page)
    {
        _page = page;
        await Cargar();
    }

    private async Task OnSearchKey(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await Buscar();
    }

    private static string FormatStatus(UnidadProductoStatus status) => status.GetDisplayName();
}