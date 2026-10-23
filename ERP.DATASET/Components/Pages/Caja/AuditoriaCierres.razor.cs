using ERP.DATA.Services.CajaService;
using ERP.TRAN.CrossLayers.API.Pos.Shifts.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Terminals.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Caja;

public partial class AuditoriaCierres
{
    [Inject] private CajaManager CajaManager { get; set; } = null!;

    private bool _loading = true;
    private string? _error;

    private List<PosShiftSummaryDto> _items = new();
    private List<PosTerminalDto> _terminals = new();
    private List<CashierAssignmentDto> _cashiers = new();

    // Filtros
    private DateTime? _fromDate;
    private DateTime? _toDate;
    private int _terminalId = 0;
    private int _cashierId = 0;

    // Paginación
    private int _pageNumber = 1;
    private int _pageSize = 20;
    private int _totalCount = 0;
    private int _totalPages => _pageSize > 0 ? (int)Math.Ceiling(_totalCount / (double)_pageSize) : 1;

    // Detalle
    private bool _showDetail;
    private bool _loadingDetail;
    private PosShiftDetailDto? _detail;

    protected override async Task OnInitializedAsync()
    {
        await LoadFiltersAsync();
        await LoadData();
    }

    private async Task LoadFiltersAsync()
    {
        try
        {
            _terminals = await CajaManager.List();
            _cashiers = await CajaManager.ListCashierStoreAssignmentsAsync();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
    }

    private async Task LoadData()
    {
        _loading = true;
        _error = null;
        try
        {
            var paged = await CajaManager.ListClosedShiftsAsync(_fromDate, _toDate, _terminalId > 0 ? _terminalId : null, _cashierId > 0 ? _cashierId : null, _pageNumber, _pageSize, CancellationToken.None);
            _items = paged.ToList();
            _totalCount = paged.TotalCount;
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task ApplyFilters()
    {
        _pageNumber = 1;
        await LoadData();
    }

    private async Task GoToPage(int page)
    {
        if (page < 1 || page > _totalPages) return;
        _pageNumber = page;
        await LoadData();
    }

    private async Task OpenDetail(int shiftId)
    {
        _showDetail = true;
        _loadingDetail = true;
        _detail = null;
        try
        {
            _detail = await CajaManager.GetShiftDetailAsync(shiftId, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _loadingDetail = false;
        }
    }

    private void CloseDetail()
    {
        _showDetail = false;
        _detail = null;
    }
}
