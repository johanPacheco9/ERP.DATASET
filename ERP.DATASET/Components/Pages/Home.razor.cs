using ERP.DATA.Services.InventarioService.AuditService;
using ERP.DATA.Services.InventarioService.Movimientos;
using ERP.DATA.Services.InventarioService.UnidadProductoService;
using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.VentasService.Payments;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using Microsoft.AspNetCore.Components;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.DATASET.Components.Pages;

public partial class Home
{
    [Inject] private ProductoBaseService ProductoService { get; set; } = null!;
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;
    [Inject] private MovimientosManager MovimientosManager { get; set; } = null!;
    [Inject] private AuditoriaService AuditService { get; set; } = null!;
    [Inject] private UnidadProductoManager UnidadProductoManager { get; set; } = null!;
    [Inject] private SaleService SaleService { get; set; } = null!;
    [Inject] private PaymentsService PaymentsService { get; set; } = null!;

    private bool _loading = true;
    private int _productos;
    private int _bodegas;
    private int _movimientos;
    private int _auditorias;
    private int _unidades;
    private int _ventas;
    private int _alertasStock;
    private decimal _saldoCartera;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var productos = await ProductoService.ListAsync(
                request: new ListProductRequest(pageNumber: 1, pageSize: 1),
                searchTerm: null,
                categoryName: null,
                stockFilter: null,
                cancellationToken: CancellationToken.None
            );

            _productos = productos.TotalCount;

            var bodegas = await WarehouseService.List(
                new ListWarehousesRequest { PageNumber = 1, PageSize = 1 },
                CancellationToken.None);
            _bodegas = bodegas.TotalCount;

            var movs = await MovimientosManager.ListMovements(
                new ListMovementsRequest(1, 500, null));
            _movimientos = movs.Count;

            var audits = await AuditService.ListAudits(CancellationToken.None);
            _auditorias = audits.Count;

            var unidades = await UnidadProductoManager.ListAsync(
                new ListUnitProductRequest(1, 1), CancellationToken.None);
            _unidades = unidades.TotalCount;

            var ventas = await SaleService.ListAsync(1, CancellationToken.None);
            _ventas = ventas.Count;

            var cartera = await PaymentsService.ListReceivables(true, CancellationToken.None);
            _saldoCartera = cartera.Sum(c => Math.Max(0, c.Balance));

            var alertas = await WarehouseService.ListStockAlerts(CancellationToken.None);
            _alertasStock = alertas.Count;
        }
        catch
        {
            // Si la BD no está disponible, mostramos ceros
        }
        finally
        {
            _loading = false;
        }
    }
}
