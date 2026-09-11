using ERP.DATA.Services.CajaService;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Caja;

public partial class Update
{
     [Parameter]
    public int Id { get; set; }

    [Inject] private CajaManager CajaManager { get; set; } = null!;

    private UpdateCajaRequest _updateForm = new();
    private bool _isLoading = true;
    private bool _isSubmitting = false;
    private string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadCajaAsync();
    }

    private async Task LoadCajaAsync()
    {
        _isLoading = true;
        try
        {
            var result = await CajaManager.GetById(Id);
            if (result.IsSuccess && result.Value != null)
            {
                var caja = result.Value;
                _updateForm = new UpdateCajaRequest
                {
                    Id = caja.Id,
                    Name = caja.Name,
                    Code = caja.Code,
                    StoreId = caja.StoreId,
                    WarehouseId = caja.WarehouseId,
                    Prefix = caja.Prefix,
                    CurrentConsecutive = caja.CurrentConsecutive,
                    DianResolutionNumber = caja.DianResolutionNumber,
                    IsActive = caja.IsActive
                };
            }
            else
            {
                _errorMessage = result.Error?.Message ?? "No se pudo cargar la información de la caja.";
            }
        }
        catch (Exception e)
        {
            _errorMessage = "Ocurrió un error inesperado al cargar la caja.";
            Console.WriteLine(e);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task UpdateCaja()
    {
        _isSubmitting = true;
        _errorMessage = null;

        try
        {
            var result = await CajaManager.Update(_updateForm);
            if (result.IsSuccess)
            {
                Navigation.NavigateTo("/cajas");
            }
            else
            {
                _errorMessage = result.Error?.Message ?? "Error al actualizar la caja.";
            }
        }
        catch (Exception e)
        {
            _errorMessage = "Ocurrió un error inesperado al guardar los cambios.";
            Console.WriteLine(e);
        }
        finally
        {
            _isSubmitting = false;
        }
    }
}