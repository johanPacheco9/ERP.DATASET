using ERP.DATA.Services.CajaService;
using ERP.TRAN.CrossLayers.API.Pos.Caja.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using ERP.TRAN.CrossLayers.API.Stores.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Caja;

public partial class Update
{
     [Parameter]
    public int Id { get; set; }

    [Inject] public CajaManager CajaManager { get; set; } = null!;
    
    private UpdateStoreRequest _request = new();
    private List<PosTerminalDto> _cajas = new();
    private bool _isSubmitting;
    private string? _errorMessage;

    // Estado del Modal de Caja Rápida
    private bool _showCajaModal;
    private bool _isSavingCaja;
    private string? _cajaModalError;
    private CreateCajaRequest _nuevaCaja = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadStoreDataAsync();
    }

    /// <summary>
    /// Carga la información general de la tienda y sus cajas asociadas desde la base de datos.
    /// </summary>
    private async Task LoadStoreDataAsync()
    {
        try
        {
            _errorMessage = null;

            // 1. Consultar la tienda junto con sus cajas utilizando tu StoresManager (o DbContext directo según prefieras)
            // Ejemplo con StoresManager (asegúrate de tener un método GetForEdit o similar, o consulta el contexto):
            var store = await StoresManager.GetByIdAsync(Id); // O tu método equivalente

            if (store == null)
            {
                _errorMessage = "La tienda solicitada no existe o fue eliminada.";
                return;
            }

            // 2. Mapear los datos al Request de actualización para que los inputs se rellenen
            _request = new UpdateStoreRequest
            {
                Name = store.Name,
                Description = store.Description,
                IsMainStore = store.IsMainStore,
                IsActive = store.IsActive
            };

            // 3. Cargar las cajas asociadas a esta tienda para la tabla
            // (Si tu método GetById o Store incluye la lista de cajas, la asignas aquí)
            // _cajas = store.Cajas.Select(c => new PosTerminalDto(...)).ToList();
        }
        catch (Exception ex)
        {
            _errorMessage = $"No se pudo cargar la información: {ex.Message}";
        }
    }

    private async Task HandleUpdate()
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
            // TODO: Llama a tu método de actualización en StoresManager
            // await StoresManager.UpdateAsync(Id, _request);
            
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

    private void OpenCreateCajaModal()
    {
        _nuevaCaja = new CreateCajaRequest 
        { 
            StoreId = Id, 
            Prefix = "POS", 
            CurrentConsecutive = 1, 
            IsActive = true 
        };
        _cajaModalError = null;
        _showCajaModal = true;
    }

    private void CloseCajaModal()
    {
        _showCajaModal = false;
    }

    private async Task SaveNewCaja()
    {
        if (string.IsNullOrWhiteSpace(_nuevaCaja.Name) || string.IsNullOrWhiteSpace(_nuevaCaja.Code))
        {
            _cajaModalError = "El nombre y el código de la caja son obligatorios.";
            return;
        }

        _isSavingCaja = true;
        _cajaModalError = null;

        try
        {
            // Amarramos la caja a la tienda actual
            _nuevaCaja.StoreId = Id;

            // Llamamos al servicio que ya tienes listo
            await CajaManager.Create(_nuevaCaja);

            CloseCajaModal();

            // Recargamos los datos para que la nueva caja aparezca inmediatamente en la tabla
            await LoadStoreDataAsync();
        }
        catch (Exception ex)
        {
            _cajaModalError = ex.Message;
        }
        finally
        {
            _isSavingCaja = false;
        }
    }
}