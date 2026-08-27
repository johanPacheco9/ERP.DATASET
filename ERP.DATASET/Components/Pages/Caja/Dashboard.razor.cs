using ERP.DATA.Services.CajaService;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ERP.DATASET.Components.Pages.Caja;

public partial class Dashboard
{
    [Inject] private CajaManager CajaManager { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    private OpenShiftRequest _request = new();
    private bool _isSubmitting;
    private string? _errorMessage;

    private async Task HandleOpenShift()
    {
        _errorMessage = null;

        if (_request.PosTerminalId <= 0)
        {
            _errorMessage = "Debe seleccionar una terminal POS válida.";
            return;
        }

        if (_request.InitialCash < 0)
        {
            _errorMessage = "La base inicial no puede ser un valor negativo.";
            return;
        }

        try
        {
            _isSubmitting = true;

            // Simulación o asignación temporal del ID del cajero logueado actual
            // _request.CajeroId = ObtenerIdUsuarioActual(); 

            await CajaManager.OpenShiftAsync(_request, default);
            
            // Redirigir a la pantalla de ventas o panel principal al abrir con éxito
            NavigationManager.NavigateTo("/ventas");
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