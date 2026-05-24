using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Movimientos;

public partial class MovimientosDashboard
{
    [Inject] private MovimientoService MovimientoService { get; set; } = null!;

    private bool _loading = true;
    private List<MovimientoDetailDto> _items = new();

    protected override async Task OnInitializedAsync()
    {
        _items = await MovimientoService.ListMovements(
            new ListMovementsRequest(1, 100, null));
        _loading = false;
    }

    private static string GetTipoClass(TipoMovimiento tipo) =>
        tipo switch
        {
            TipoMovimiento.Entrada => "inv-badge--ok",
            TipoMovimiento.Salida => "inv-badge--warn",
            _ => "inv-badge--muted"
        };
}
