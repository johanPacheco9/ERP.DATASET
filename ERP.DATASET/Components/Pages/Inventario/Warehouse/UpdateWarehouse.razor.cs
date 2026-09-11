using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.VentasService.Stores;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Stores.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Warehouse;

public partial class UpdateWarehouse : IDisposable
{
    [Parameter]
    public int Id { get; set; }

    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private StoresManager StoreService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    
    private UpdateWarehouseForm _updateForm = new();
    
    private List<StoreSummaryDto> _tiendas = new();

    private readonly CancellationTokenSource _cts = new();
    private CancellationToken CancellationToken => _cts.Token;

    private WarehouseDetailDTO? _bodega;
    private string? _mensaje;
    private bool _guardando;

    protected override async Task OnInitializedAsync()
    {
        await LoadWarehouseAsync();
        await LoadStoresAsync();

    }

    private async Task LoadWarehouseAsync()
    {
        try
        {
            var bodega = await WarehouseService.GetBodegaByIdAsync(Id, CancellationToken);
            
            if (bodega == null)
            {
                _mensaje = "Bodega no encontrada.";
                return;
            }

            _bodega = bodega;
            
            // Poblamos el form con los datos actuales
            _updateForm = new UpdateWarehouseForm
            {
                Id = bodega.Id,
                Name = bodega.Nombre,
                Code = bodega.Code,
                Description = bodega.Descripcion,
                Ubication = bodega.Ubicacion,
                MaxCapacity = (decimal)bodega.Max_Capacity,
                Type = bodega.Type
            };
        }
        catch (Exception ex)
        {
            _mensaje = $"Error al cargar la bodega: {ex.Message}";
        }
    }

    private async Task LoadStoresAsync()
    {
        try
        {
            var request = new ListStoresRequest(pageNumber: 1, pageSize: 100);
            var result = await StoreService.List(request, null, CancellationToken);
            _tiendas = result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando tiendas: {ex.Message}");
        }
    }

    private async Task Update()
    {
        _guardando = true;
        _mensaje = null;
        try
        {
            var request = new UpdateWarehouseRequest
            {
                Id = Id,
                Code = _updateForm.Code,
                Max_Capacity = _updateForm.MaxCapacity,
                Name = _updateForm.Name,
                Description = _updateForm.Description,
                Ubication = _updateForm.Ubication,
                StoreId = _updateForm.StoreId,
                Type = _updateForm.Type
            };

            await WarehouseService.UpdateBodega(request, CancellationToken);
            _mensaje = "Bodega actualizada correctamente.";
            
            await Task.Delay(1000);
            NavigationManager.NavigateTo("/warehouses");
        }
        catch (Exception ex)
        {
            _mensaje = $"Error al guardar: {ex.Message}";
        }
        finally
        {
            _guardando = false;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}