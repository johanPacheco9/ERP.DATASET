using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using Microsoft.AspNetCore.Components;
namespace ERP.DATASET.Components.Pages.Parametrizacion.Productos;

public partial class Create
{
    [Inject]
    private CategoriaService CategoriaSvc { get; set; } = null!;
    
    private List<CategoriaDetailDto> _categorias = new();
    
    private readonly CancellationTokenSource _cts = new();
    protected CancellationToken CancellationToken => _cts.Token;

    protected async override void OnInitialized()
    {
        try
        {
            await GetCategorias();
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }

    private CreateProductoRequest _request = new() 
    { 
        PorcentajeIVA = 0.19m, // Valor por defecto
        Unidad_Medida = "UND" 
    };
    
    private bool _procesando = false;
    private string? _errorMensaje;

    private async Task HandleSubmit()
    {
        _procesando = true;
        _errorMensaje = null;

        try
        {
            int id = await ProductoBaseSvc.AddProductoAsync(_request);
            // Navegar al detalle o a la lista
            Nav.NavigateTo($"/inventario/productos/detalle/{id}");
        }
        catch (Exception ex)
        {
            _errorMensaje = ex.Message;
        }
        finally
        {
            _procesando = false;
        }
    }

    private async Task GetCategorias()
    {
        var request = new ListCategoriasRequest { PageSize = -1 };
        var response = await CategoriaSvc.List(request, CancellationToken);

        if (response.Count == 0)
        {
            _errorMensaje = "Aún no hay categorías registradas.";
            _categorias = new();
            return;
        }

        _categorias = response;
    }
    private void Volver() => Nav.NavigateTo("/inventario/productos");
}