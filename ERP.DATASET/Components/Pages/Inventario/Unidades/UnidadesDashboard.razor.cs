using ERP.DATA.Services.InventarioService.ProductService;
using ERP.DATA.Services.InventarioService.UnitProductService;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ERP.DATASET.Components.Pages.Inventario.Unidades;

public partial class UnidadesDashboard
{
    [Inject] private UnitProductService UnitProductService { get; set; } = null!;
    [Inject] private ProductService ProductService { get; set; } = null!;

    private bool _loading = true;
    private string? _error;
    private string _search = "";
    private int _page = 1;
    private const int PageSize = 50;
    private int _total;
    private int _totalPages;
    private int _totalCatalogo;
    private List<UnitProductDetailDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var catalogo = await ProductService.ListAsync(new ListProductRequest(1, 1), CancellationToken.None);
            _totalCatalogo = catalogo.TotalCount;
        }
        catch { /* opcional */ }

        await Cargar();
    }

    private async Task Cargar()
    {
        _loading = true;
        _error = null;
        try
        {
            var request = new ListUnitProductRequest(_page, PageSize)
            {
                Search = string.IsNullOrWhiteSpace(_search) ? null : _search.Trim()
            };

            var result = await UnitProductService.ListAsync(request, CancellationToken.None);
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

    private static string FormatStatus(ProductoStatus status) => status.GetDisplayName();
}
