using ERP.DATA.Services.Inventario.ProveedorService;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Proveedores;

public partial class ProveedoresDashboard
{
    [Inject] private SupplierService SupplierService { get; set; } = null!;

    private bool _loading = true;
    private string? _error;
    private List<Supplier> _items = new();

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        _error = null;
        try
        {
            _items = await SupplierService.ListAsync(100, CancellationToken.None);
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
}
