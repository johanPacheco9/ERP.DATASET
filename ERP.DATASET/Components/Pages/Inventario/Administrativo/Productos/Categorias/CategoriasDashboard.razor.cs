using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos.Categorias;

public partial class CategoriasDashboard
{
    [Inject] private CategoriaService CategoriaService { get; set; } = null!;

    private bool _loading = true;
    private List<CategoriaDetailDto> _items = new();
    private int _total;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await CategoriaService.List(
                new ListCategoriasRequest(pageNumber: 1, pageSize: -1),
                CancellationToken.None);

            _items = result.ToList();
            _total = result.TotalCount;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TIPO: {ex.GetType().Name}");
            Console.WriteLine($"MENSAJE: {ex.Message}");
            Console.WriteLine($"INNER: {ex.InnerException?.Message}");
            Console.WriteLine($"STACK: {ex.StackTrace}");
        }
        finally
        {
            _loading = false; // siempre se ejecuta, con o sin datos
        }
    }
}
