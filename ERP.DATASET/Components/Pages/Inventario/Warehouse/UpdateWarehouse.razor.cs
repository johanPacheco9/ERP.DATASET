using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATASET.Components.Pages.Inventario.NewFolder;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Warehouse;

public partial class UpdateWarehouse : IDisposable
{
    [Inject] private WarehouseService WarehouseService { get; set; } = null!;

    private readonly CancellationTokenSource _cts = new();
    protected CancellationToken CancellationToken => _cts.Token;

    private List<EditableWarehouseRow> _rows = new();
    private string? _mensaje;
    private bool _guardando;

    protected override async Task OnInitializedAsync() => await LoadWarehouses();

    private async Task LoadWarehouses()
    {
        var response = await WarehouseService.List(
            new ListWarehousesRequest { PageNumber = 1, PageSize = 50 },
            CancellationToken);

        _rows = response
            .Select(dto => new EditableWarehouseRow(UpdateWarehouseForm.FromSummaryDto(dto)))
            .ToList();
    }

    private async Task SaveChanges()
    {
        var modified = _rows.Where(r => r.IsModified).Select(r => r.Form.ToRequest()).ToList();
        if (!modified.Any())
        {
            _mensaje = "No hay cambios pendientes.";
            return;
        }

        _guardando = true;
        _mensaje = null;
        try
        {
            foreach (var req in modified)
                await WarehouseService.UpdateBodega(req, CancellationToken);
            _mensaje = $"Se guardaron {modified.Count} bodega(s).";
            await LoadWarehouses();
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

    private void OnRowValidSubmit(EditableWarehouseRow row) => row.MarkValid();

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
