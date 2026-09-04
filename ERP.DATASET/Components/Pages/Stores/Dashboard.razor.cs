using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Stores.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Pagination;

namespace ERP.DATASET.Components.Pages.Stores;

public partial class Dashboard
{
    private List<StoreSummaryDto> _stores = [];
    private bool _isLoading = true;
    private string? _searchTerm;
    private int _pageNumber = 1;
    private int _pageSize = 10;

    protected override async Task OnInitializedAsync()
    {
        await LoadStoresAsync();
    }

    private async Task LoadStoresAsync()
    {
        _isLoading = true;
        try
        {
            var request = new ListStoresRequest { PageNumber = _pageNumber, PageSize = _pageSize };
            var result= await StoresManager.List(request, _searchTerm, default);

            if (result.TotalCount > 0)
            {
                _stores = result;
            }
        }
        catch (Exception)
        {
            // Manejo de errores de carga
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task FilterChanged()
    {
        _pageNumber = 1;
        await LoadStoresAsync();
    }

    private void NavigateToCreate()
    {
        Navigation.NavigateTo("/tiendas/nueva");
    }

    private void NavigateToEdit(int id)
    {
        Navigation.NavigateTo($"/tiendas/editar/{id}");
    }
}