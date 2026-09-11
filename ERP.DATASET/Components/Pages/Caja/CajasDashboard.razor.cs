using ERP.DATA.Repositories;
using ERP.DATA.Services.CajaService;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATASET.Components.Pages.Caja;

public partial class CajasDashboard
{
    [Inject] private CajaManager CajaManager { get; set; } = null!;
    [Inject] private MainDataContext Context { get; set; } = null!;

    private bool _isLoading = true;
    private string _activeTab = "terminales";
    private string? _errorMessage;
    private string? _successMessage;

    private List<PosTerminalDto> _terminals = new();
    private List<CashierAssignmentDto> _cashiers = new();
    private List<Store> _allStores = new();

    private string _searchTerminal = string.Empty;
    private string _searchCashier = string.Empty;

    // Modal de Asignación de Sucursales
    private bool _showAssignModal;
    private bool _isSavingAssignment;
    private string? _modalError;
    private CashierAssignmentDto? _selectedCashier;
    private HashSet<int> _selectedStoreIds = new();

    private IEnumerable<PosTerminalDto> _filteredTerminals =>
        string.IsNullOrWhiteSpace(_searchTerminal)
            ? _terminals
            : _terminals.Where(t =>
                (t.Name != null && t.Name.Contains(_searchTerminal, StringComparison.OrdinalIgnoreCase)) ||
                (t.Code != null && t.Code.Contains(_searchTerminal, StringComparison.OrdinalIgnoreCase)) ||
                (t.StoreName != null && t.StoreName.Contains(_searchTerminal, StringComparison.OrdinalIgnoreCase))
            );

    private IEnumerable<CashierAssignmentDto> _filteredCashiers =>
        string.IsNullOrWhiteSpace(_searchCashier)
            ? _cashiers
            : _cashiers.Where(c =>
                (c.NombreCompleto != null && c.NombreCompleto.Contains(_searchCashier, StringComparison.OrdinalIgnoreCase)) ||
                (c.Email != null && c.Email.Contains(_searchCashier, StringComparison.OrdinalIgnoreCase)) ||
                (c.UserName != null && c.UserName.Contains(_searchCashier, StringComparison.OrdinalIgnoreCase))
            );

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private void SetTab(string tab)
    {
        _activeTab = tab;
        _errorMessage = null;
        _successMessage = null;
    }

    private async Task LoadDataAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        try
        {
            _terminals = await CajaManager.List();
            _cashiers = await CajaManager.ListCashierStoreAssignmentsAsync();
            _allStores = await Context.Store.Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error al cargar datos: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void OpenAssignModal(CashierAssignmentDto cashier)
    {
        _selectedCashier = cashier;
        _selectedStoreIds = cashier.AssignedStores.Select(s => s.StoreId).ToHashSet();
        _modalError = null;
        _showAssignModal = true;
    }

    private void CloseAssignModal()
    {
        _showAssignModal = false;
        _selectedCashier = null;
    }

    private void ToggleStoreSelection(int storeId, object? isChecked)
    {
        if (isChecked is bool selected && selected)
        {
            _selectedStoreIds.Add(storeId);
        }
        else
        {
            _selectedStoreIds.Remove(storeId);
        }
    }

    private async Task SaveCashierAssignments()
    {
        if (_selectedCashier == null) return;

        _isSavingAssignment = true;
        _modalError = null;

        try
        {
            var currentStoreIds = _selectedCashier.AssignedStores.Select(s => s.StoreId).ToHashSet();

            // 1. Agregar las nuevas asignaciones seleccionadas
            foreach (var storeId in _selectedStoreIds)
            {
                if (!currentStoreIds.Contains(storeId))
                {
                    await CajaManager.AssignCashierToStoreAsync(_selectedCashier.UsuarioId, storeId, isDefault: false);
                }
            }

            // 2. Remover las asignaciones desmarcadas
            foreach (var storeId in currentStoreIds)
            {
                if (!_selectedStoreIds.Contains(storeId))
                {
                    await CajaManager.RemoveCashierFromStoreAsync(_selectedCashier.UsuarioId, storeId);
                }
            }

            _successMessage = $"Sucursales actualizadas para {_selectedCashier.NombreCompleto} exitosamente.";
            CloseAssignModal();

            // Recargar datos
            _cashiers = await CajaManager.ListCashierStoreAssignmentsAsync();
            _terminals = await CajaManager.List();
        }
        catch (Exception ex)
        {
            _modalError = ex.Message;
        }
        finally
        {
            _isSavingAssignment = false;
        }
    }
}
