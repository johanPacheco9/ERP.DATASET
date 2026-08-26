using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using Microsoft.AspNetCore.Components;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos;

public partial class CreateBaseProduct : IDisposable
{
    [Inject] private CategoriaService CategoriaService { get; set; } = null!;
    [Inject] private ProductoBaseService ProductoBaseService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private readonly CancellationTokenSource _cts = new();
    private List<CategoriaDetailDto> _categorias = [];
    private string? mensajeError;
    private string? mensajeOk;
    private bool cargando;

    private CreateProductoRequest request = new()
    {
        Unidad_Medida = "UND",
        PorcentajeIVA = 0.19m
    };

    protected override async Task OnInitializedAsync()
    {
        await GetCategorias();
    }

    private async Task GetCategorias()
    {
        try
        {
            cargando = true;
            mensajeError = null;

            var response = await CategoriaService.List(
                new ListCategoriasRequest { PageSize = -1 },
                _cts.Token);

            _categorias = response.ToList();
            if (_categorias.Count == 0)
                mensajeError = "Aun no hay categorias registradas.";
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            mensajeError = $"Error al cargar categorias: {ex.Message}";
        }
        finally
        {
            cargando = false;
        }
    }

    private async Task HandleSubmit()
    {
        mensajeError = null;
        mensajeOk = null;

        try
        {
            cargando = true;
            await ProductoBaseService.AddProductoAsync(request, _cts.Token);
            mensajeOk = $"Producto {request.Nombre} creado correctamente.";
            Navigation.NavigateTo("/ProductosDashboard");
        }
        catch (Exception ex)
        {
            mensajeError = ex.Message;
        }
        finally
        {
            cargando = false;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
