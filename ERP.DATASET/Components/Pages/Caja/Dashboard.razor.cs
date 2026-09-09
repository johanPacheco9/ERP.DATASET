using ERP.DATA.Services.CajaService;
using ERP.DATA.Services.UserService;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using ERP.TRAN.CrossLayers.API.Users.Enums;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Caja;

public partial class Dashboard
{
    [Inject] private CajaManager CajaManager { get; set; } = null!;
    [Inject] private UserManager UserManager { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private bool _isLoading = true;
    private int _currentUserId = 1;
    private UserRole _currentUserRole = UserRole.Cashier;

    private ActiveUserShiftDto? _activeShift;
    private List<PosTerminalDto> _availableTerminals = new();
    private OpenShiftRequest _request = new();

    private bool _isSubmitting;
    private string? _errorMessage;
    private string? _successMessage;

    // Cierre de Caja Modal
    private bool _showCloseModal;
    private bool _isClosing;
    private string? _closeModalError;
    private CloseShiftRequest _closeRequest = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCajaStateAsync();
    }

    private async Task LoadCajaStateAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        try
        {
            var userId = await UserManager.GetUserId();
            var role = await UserManager.GetRole();

            _currentUserId = userId ?? 1;
            _currentUserRole = role ?? UserRole.Cashier;

            // 1. Verificar si ya tiene un turno abierto
            _activeShift = await CajaManager.GetActiveShiftForUserAsync(_currentUserId);

            // 2. Si no tiene turno abierto, cargar las cajas disponibles
            if (_activeShift == null)
            {
                _availableTerminals = await CajaManager.GetAvailableTerminalsForUserAsync(_currentUserId, _currentUserRole);
                
                // Si solo hay una terminal libre disponible, preseleccionarla
                var freeTerminals = _availableTerminals.Where(t => !t.HasActiveShift).ToList();
                if (freeTerminals.Count == 1)
                {
                    _request.PosTerminalId = freeTerminals.First().Id;
                }
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error al cargar estado de caja: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task HandleOpenShift()
    {
        _errorMessage = null;

        if (_request.PosTerminalId <= 0)
        {
            _errorMessage = "Debe seleccionar una terminal POS válida para abrir el turno.";
            return;
        }

        if (_request.InitialCash < 0)
        {
            _errorMessage = "La base inicial de efectivo no puede ser un valor negativo.";
            return;
        }

        try
        {
            _isSubmitting = true;

            await CajaManager.OpenShiftAsync(_request, _currentUserId, default);

            // Redirigir a la pantalla de ventas
            NavigationManager.NavigateTo("/ventas/nueva");
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

    private void OpenCloseShiftModal()
    {
        if (_activeShift == null) return;

        _closeRequest = new CloseShiftRequest
        {
            PosShiftId = _activeShift.ShiftId,
            ActualCash = 0,
            Notes = null
        };
        _closeModalError = null;
        _showCloseModal = true;
    }

    private void CloseCloseShiftModal()
    {
        _showCloseModal = false;
    }

    private async Task ExecuteCloseShift()
    {
        _closeModalError = null;

        if (_closeRequest.ActualCash < 0)
        {
            _closeModalError = "El efectivo real contado no puede ser negativo.";
            return;
        }

        try
        {
            _isClosing = true;

            var resultado = await CajaManager.CierreCaja(_closeRequest, default);

            _showCloseModal = false;
            _successMessage = $"Turno #{resultado.Id} cerrado exitosamente. Efectivo esperado: {resultado.TotalExpectedCash:C0}, Contado: {resultado.ActualCash:C0}, Diferencia: {resultado.Difference:C0}";
            
            // Recargar estado
            await LoadCajaStateAsync();
        }
        catch (Exception ex)
        {
            _closeModalError = ex.Message;
        }
        finally
        {
            _isClosing = false;
        }
    }
}