using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;

namespace ERP.DATASET.Components.Pages.Stores;

public partial class Create
{
    private CreateStoreRequest _request = new() { IsActive = true };
    private bool _isSubmitting;
    private string? _errorMessage;

    private async Task HandleCreate()
    {
        _errorMessage = null;

        if (string.IsNullOrWhiteSpace(_request.Name))
        {
            _errorMessage = "El nombre de la tienda es obligatorio.";
            return;
        }

        try
        {
            _isSubmitting = true;

            // TODO: Invoca tu método de creación en el manager
            // await StoresManager.CreateAsync(_request);

            // Redirige directo al listado al terminar con éxito
            Navigation.NavigateTo("/tiendas");
        }
        catch (Exception ex)
        {
            _errorMessage = ex.Message;
        }
        finally
        {
            _isSubmitting = false;
        }
    }
}