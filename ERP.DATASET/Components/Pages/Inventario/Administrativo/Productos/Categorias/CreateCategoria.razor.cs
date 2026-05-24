using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Administrativo.Productos.Categorias;

public partial class CreateCategoria
{
    [Inject] private CategoriaService CategoriaService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    // El modelo que está vinculado al EditForm en el .razor
    private CreateCategoryForm _CreateForm = new();
    
    private bool _estaCargando = false;
    private string? _mensajeError;

    private async Task HandleValidSubmit()
    {
        _estaCargando = true;
        _mensajeError = null;

        var request = new CreateCategoriaRequest
        {
            Nombre = _CreateForm.Name,
            Descripcion = _CreateForm.Description,
            codigo = _CreateForm.Code
        };
        
        var resultado = await CategoriaService.AddCategoriasAsync(request);

        if (resultado != null)
        {
            // Éxito: Redirigir
            Navigation.NavigateTo("/categorias");
        }
        else
        {
            // Si el servicio devuelve null es porque falló la validación interna o la DB
            _mensajeError = "No se pudo guardar la categoría. Verifique que los datos sean correctos.";
        }

        _estaCargando = false;
    }

    private void Cancelar() => Navigation.NavigateTo("/categorias");
}