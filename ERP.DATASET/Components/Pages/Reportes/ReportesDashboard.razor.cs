using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.ReportesService;
using ERP.DATA.Services.VentasService.Stores;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using ERP.TRAN.CrossLayers.API.Pos.Stores.Requests;
using ERP.TRAN.CrossLayers.API.Reportes.Requests;
using ERP.TRAN.CrossLayers.API.Reportes.Responses;
using ERP.TRAN.CrossLayers.API.Stores.Responses;
using ERP.TRAN.CrossLayers.Core.Interfaces.ReportesServices;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Reportes;

public partial class ReportesDashboard : ComponentBase
{
    [Inject] private IReportesService ReportesService { get; set; } = null!;
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private StoresManager StoresManager { get; set; } = null!;

    // Pestaña Activa: 1 = Utilidad, 2 = Rotacion, 3 = Productos Top, 4 = Cajas
    private int _activeTab = 1;
    private bool _loading = true;
    private string? _error;

    // Filtros Comunes
    private DateTime _fechaDesde = DateTime.Today.AddDays(-30);
    private DateTime _fechaHasta = DateTime.Today;
    private string _presetSeleccionado = "mes";
    private int _selectedStoreId = 0;
    private int _selectedWarehouseId = 0;

    // Filtros Específicos
    private int _topProductos = 10;
    private string _ordenarTopPor = "cantidad"; // "cantidad" o "total"
    private string _filtroRotacionClasificacion = "Todos";
    private string _searchRotacion = "";
    private string _searchUtilidad = "";

    // Listas para Selectores
    private List<StoreSummaryDto> _tiendas = new();
    private List<WarehouseSummaryDto> _bodegas = new();

    // DTOs de Reportes
    private ReporteUtilidadDto? _reporteUtilidad;
    private ReporteRotacionInventarioDto? _reporteRotacion;
    private ReporteProductosMasVendidosDto? _reporteTopProductos;
    private ReporteCajasMasVendedorasDto? _reporteCajas;

    // Modal de Capas FIFO
    private bool _mostrarModalCapas = false;
    private List<CapaFifoDisponibleDto> _capasFiltradasModal = new();
    private string _modalProductoNombre = "";

    protected override async Task OnInitializedAsync()
    {
        await CargarFiltrosIniciales();
        await CargarReporteActivo();
    }

    private async Task CargarFiltrosIniciales()
    {
        try
        {
            var tiendasResult = await StoresManager.List(new ListStoresRequest { PageNumber = 1, PageSize = 100 }, null, default);
            _tiendas = tiendasResult?.ToList() ?? new();

            var bodegasResult = await WarehouseService.List(new ListWarehousesRequest { PageNumber = 1, PageSize = 100 }, default);
            _bodegas = bodegasResult?.ToList() ?? new();
        }
        catch (Exception ex)
        {
            _error = $"Error al cargar tiendas/bodegas: {ex.Message}";
        }
    }

    private void SetPreset(string preset)
    {
        _presetSeleccionado = preset;
        var hoy = DateTime.Today;

        switch (preset)
        {
            case "hoy":
                _fechaDesde = hoy;
                _fechaHasta = hoy;
                break;
            case "7dias":
                _fechaDesde = hoy.AddDays(-7);
                _fechaHasta = hoy;
                break;
            case "mes":
                _fechaDesde = new DateTime(hoy.Year, hoy.Month, 1);
                _fechaHasta = hoy;
                break;
            case "anio":
                _fechaDesde = new DateTime(hoy.Year, 1, 1);
                _fechaHasta = hoy;
                break;
        }

        _ = CargarReporteActivo();
    }

    private async Task CambiarPestana(int tab)
    {
        _activeTab = tab;
        await CargarReporteActivo();
    }

    private async Task CargarReporteActivo()
    {
        _loading = true;
        _error = null;

        try
        {
            var storeId = _selectedStoreId > 0 ? (int?)_selectedStoreId : null;
            var warehouseId = _selectedWarehouseId > 0 ? (int?)_selectedWarehouseId : null;
            var desde = _fechaDesde.Date;
            var hasta = _fechaHasta.Date.AddDays(1).AddTicks(-1);

            switch (_activeTab)
            {
                case 1:
                    var reqUtilidad = new ReporteUtilidadRequest
                    {
                        FechaInicio = desde,
                        FechaFin = hasta,
                        StoreId = storeId,
                        WarehouseId = warehouseId
                    };
                    var resUtilidad = await ReportesService.ObtenerReporteUtilidadAsync(reqUtilidad);
                    if (resUtilidad.IsSuccess) _reporteUtilidad = resUtilidad.Value;
                    else _error = resUtilidad.Error.Message;
                    break;

                case 2:
                    var reqRotacion = new ReporteRotacionInventarioRequest
                    {
                        Desde = desde,
                        Hasta = hasta,
                        BranchId = storeId,
                        WarehouseId = warehouseId,
                        Clasificacion = _filtroRotacionClasificacion,
                        Search = _searchRotacion
                    };
                    var resRotacion = await ReportesService.ObtenerReporteRotacionInventarioAsync(reqRotacion);
                    if (resRotacion.IsSuccess) _reporteRotacion = resRotacion.Value;
                    else _error = resRotacion.Error.Message;
                    break;

                case 3:
                    var reqTop = new ReporteProductosMasVendidosRequest
                    {
                        FechaInicio = desde,
                        FechaFin = hasta,
                        StoreId = storeId,
                        WarehouseId = warehouseId,
                        Top = _topProductos,
                        OrdenarPorTotal = _ordenarTopPor == "total"
                    };
                    var resTop = await ReportesService.ObtenerReporteProductosMasVendidosAsync(reqTop);
                    if (resTop.IsSuccess) _reporteTopProductos = resTop.Value;
                    else _error = resTop.Error.Message;
                    break;

                case 4:
                    var reqCajas = new ReporteCajasMasVendedorasRequest
                    {
                        FechaInicio = desde,
                        FechaFin = hasta,
                        StoreId = storeId
                    };
                    var resCajas = await ReportesService.ObtenerReporteCajasMasVendedorasAsync(reqCajas);
                    if (resCajas.IsSuccess) _reporteCajas = resCajas.Value;
                    else _error = resCajas.Error.Message;
                    break;
            }
        }
        catch (Exception ex)
        {
            _error = $"Error al obtener datos del reporte: {ex.Message}";
        }
        finally
        {
            _loading = false;
        }
    }

    private void AbrirModalCapas(int varianteId, string nombreProducto)
    {
        if (_reporteRotacion == null) return;
        _modalProductoNombre = nombreProducto;
        _capasFiltradasModal = _reporteRotacion.CapasFifo
            .Where(c => c.ProductoVarianteId == varianteId)
            .ToList();
        _mostrarModalCapas = true;
    }

    private void CerrarModalCapas()
    {
        _mostrarModalCapas = false;
        _capasFiltradasModal.Clear();
    }

    private IEnumerable<ItemUtilidadProductoDto> ItemsUtilidadFiltrados =>
        _reporteUtilidad == null ? Enumerable.Empty<ItemUtilidadProductoDto>() :
        string.IsNullOrWhiteSpace(_searchUtilidad) ? _reporteUtilidad.DesglosePorProducto :
        _reporteUtilidad.DesglosePorProducto.Where(p =>
            p.NombreProducto.Contains(_searchUtilidad, StringComparison.OrdinalIgnoreCase) ||
            p.Sku.Contains(_searchUtilidad, StringComparison.OrdinalIgnoreCase));
}
