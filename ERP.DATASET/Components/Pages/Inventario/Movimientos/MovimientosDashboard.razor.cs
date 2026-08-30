using ERP.DATA.Repositories;
using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATASET.Components.Pages.Inventario.Movimientos;

public partial class MovimientosDashboard
{
    [Inject] private MovimientoService MovimientoService { get; set; } = null!;
    [Inject] private MainDataContext Context { get; set; } = null!;

    private bool _loading = true;
    private List<MovimientoDetailDto> _items = new();
    // Control del Modal de Traspaso
    private bool _showTransferModal = false;
    private bool _loadingUnits = false;
    private bool _isSubmitting = false;
    private string? _errorMessage;
    private string _filtroSkuSerial = string.Empty;

    private RegistrarMovimientoRequest _transferRequest = new();
    private List<TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory.Warehouse> _warehouses = new();
    private List<UnidadProducto> _availableUnits = new();
    
    
    [Inject] private NavigationManager Navigation { get; set; } = null!;

// Método para ir a la vista de detalle
    private void VerDetalle(int movimientoId)
    {
        Navigation.NavigateTo($"/inventario/movimientos/{movimientoId}");
    }
    
    // Lista cacheada de tipos disponibles
    private List<KeyValuePair<int, string>> _tiposMovimientoDisponibles = new();

    private int _tipoMovimientoSeleccionadoInt
    {
        get => (int)_transferRequest.TipoMovimiento;
        set 
        {
            _transferRequest.TipoMovimiento = (TipoMovimiento)value;
            StateHasChanged();
        }
    }

    private IEnumerable<UnidadProducto> UnidadesFiltradas => 
        string.IsNullOrWhiteSpace(_filtroSkuSerial) 
            ? _availableUnits 
            : _availableUnits.Where(u => 
                (u.ProductoVariante?.SKU != null && u.ProductoVariante.SKU.Contains(_filtroSkuSerial, StringComparison.OrdinalIgnoreCase)) ||
                (u.SerialNumber != null && u.SerialNumber.Contains(_filtroSkuSerial, StringComparison.OrdinalIgnoreCase)) ||
                (u.ProductoVariante?.ProductoBase?.Name != null && u.ProductoVariante.ProductoBase.Name.Contains(_filtroSkuSerial, StringComparison.OrdinalIgnoreCase))
            );

    protected override async Task OnInitializedAsync()
    {
        _tiposMovimientoDisponibles = EnumExtensions.ToSelectList<TipoMovimiento>()
            .Where(x => 
            {
                var tipo = (TipoMovimiento)x.Key;
                return tipo == TipoMovimiento.Transferencia;
            })
            .ToList();
            
        await LoadMovementsAsync();
    }

    private async Task LoadMovementsAsync()
    {
        _loading = true;
        _items = await MovimientoService.ListMovements(
            new ListMovementsRequest(1, 100, null));
        _loading = false;
    }

    private async Task AbrirModalTraspaso()
    {
        var primerTipoValido = _tiposMovimientoDisponibles.Any() 
            ? _tiposMovimientoDisponibles.First().Key 
            : (int)TipoMovimiento.Transferencia;

        _transferRequest = new RegistrarMovimientoRequest 
        { 
            ProductIds = new List<int>(),
            TipoMovimiento = (TipoMovimiento)primerTipoValido
        };
    
        _errorMessage = null;
        _filtroSkuSerial = string.Empty;
        _availableUnits.Clear();

        _warehouses = await Context.Warehouse.Where(w => w.IsActive).ToListAsync();
        _showTransferModal = true;
    }

    private void CerrarModalTraspaso()
    {
        _showTransferModal = false;
    }

    private async Task OnOrigenChanged()
    {
        _transferRequest.ProductIds.Clear();
        _availableUnits.Clear();
        _filtroSkuSerial = string.Empty;

        if (_transferRequest.OriginWarehouseId > 0)
        {
            _loadingUnits = true;
            _availableUnits = await Context.UnidadesProductos
                .Include(u => u.ProductoVariante)
                .ThenInclude(v => v.ProductoBase)
                .Where(u => u.BodegaId == _transferRequest.OriginWarehouseId && u.Status == UnidadProductoStatus.Available)
                .ToListAsync();
            _loadingUnits = false;
        }
    }

    private void ToggleProductSelection(int unitId, object? isChecked)
    {
        if (isChecked is bool selected && selected)
        {
            if (!_transferRequest.ProductIds.Contains(unitId))
                _transferRequest.ProductIds.Add(unitId);
        }
        else
        {
            _transferRequest.ProductIds.Remove(unitId);
        }
    }

    private async Task EjecutarTraspaso()
    {
        _errorMessage = null;
        if (_transferRequest.OriginWarehouseId == 0 || _transferRequest.DestinationWarehouseId == null || _transferRequest.DestinationWarehouseId == 0)
        {
            _errorMessage = "Debe seleccionar la bodega de origen y destino.";
            return;
        }

        if (_transferRequest.ProductIds == null || !_transferRequest.ProductIds.Any())
        {
            _errorMessage = "Debe seleccionar al menos una unidad para traspasar.";
            return;
        }

        _isSubmitting = true;
        var result = await MovimientoService.RegistrarMovimientoInventario(_transferRequest, default);
        _isSubmitting = false;

        if (result.IsSuccess)
        {
            CerrarModalTraspaso();
            await LoadMovementsAsync();
        }
        else
        {
            _errorMessage = result.Error.Message;
        }
    }

    private static string GetTipoClass(TipoMovimiento tipo) =>
        tipo switch
        {
            TipoMovimiento.Entrada or TipoMovimiento.EntradaTransferencia => "inv-badge--ok",
            TipoMovimiento.Salida or TipoMovimiento.SalidaTransferencia => "inv-badge--warn",
            _ => "inv-badge--muted"
        };
}