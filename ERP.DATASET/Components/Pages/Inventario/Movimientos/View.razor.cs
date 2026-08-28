using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Movimientos;

public partial class View
{
    [Parameter] public int Id { get; set; }

    [Inject] private MovimientoService MovimientoService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private bool _loading = true;
    private MovimientoDetailDto? _movimiento;

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        _movimiento = await MovimientoService.GetMovementByIdAsync(Id);
        _loading = false;
    }

    private void Volver()
    {
        Navigation.NavigateTo("/inventario/movimientos");
    }
}