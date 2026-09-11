using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.VentasService.Stores;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Stores.Responses;
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Warehouse;

public partial class AddWarehouse : ComponentBase
{
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private StoresManager StoreService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private CreateBodegaFormModel _createForm { get; set; } = new();
    private List<StoreSummaryDto> _tiendas = new();
    private bool _loading;
    private string? _error;

    protected override async Task OnInitializedAsync()
    {
        await CargarTiendas();
    }

    private async Task CargarTiendas()
    {
        try
        {
            var request = new ListStoresRequest(pageNumber: 1, pageSize: 100);
            var result = await StoreService.List(request, null, CancellationToken.None);
            _tiendas = result;
        }
        catch (Exception ex)
        {
            _error = "No se pudieron cargar las tiendas disponibles.";
            Console.WriteLine(ex);
        }
    }

    private async Task Create()
    {
        _loading = true;
        _error = null;

        var request = new CreateBodegaRequest
        {
            Nombre = _createForm.Nombre,
            Code = _createForm.Code,
            Descripcion = _createForm.Descripcion,
            Ubicacion = _createForm.Ubicacion,
            CapacidadMaxima = _createForm.CapacidadMaxima,
            IsActive = _createForm.IsActive,
            TipoBodega = _createForm.TipoBodega,
            storeId = _createForm.storeId
        };

        try
        {
            await WarehouseService.AddBodegaAsync(request, CancellationToken.None);
            NavigationManager.NavigateTo("/warehouses");
        }
        catch (Exception e)
        {
            _error = $"Error al registrar la bodega: {e.Message}";
            Console.WriteLine(e);
        }
        finally
        {
            _loading = false;
        }
    }

    public class CreateBodegaFormModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public bool IsActive { get; set; } = true;
        public WarehouseType TipoBodega { get; set; }
        public int storeId { get; set; }
    }
}