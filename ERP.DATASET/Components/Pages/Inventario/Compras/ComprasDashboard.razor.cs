using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Responses;
using ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;

namespace ERP.DATASET.Components.Pages.Inventario.Compras;

public partial class ComprasDashboard
{
    private List<OrdenCompraSummaryDto> Ordenes = new();
    private List<ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory.Proveedor> Proveedores = new();
    private bool Cargando = true;

    private OrdenCompraStatus? FiltroStatus;
    private int? FiltroProveedorId;

    protected override async Task OnInitializedAsync()
    {
        Proveedores = await ProveedorService.ListAsync(100, default);
        await CargarOrdenes();
    }

    private async Task CargarOrdenes()
    {
        Cargando = true;
        Ordenes = await OrdenesDeCompraManager.GetList(FiltroStatus, FiltroProveedorId, null, null, default);
        Cargando = false;
    }

    private string GetStatusCssClass(string status) => status switch
    {
        "Draft" => "bg-slate-100 text-slate-700",
        "PendingApproval" => "bg-amber-100 text-amber-800",
        "Approved" => "bg-sky-100 text-sky-800",
        "Sent" => "bg-indigo-100 text-indigo-800",
        "PartiallyReceived" => "bg-purple-100 text-purple-800",
        "Received" => "bg-emerald-100 text-emerald-800",
        "Finalized" => "bg-blue-100 text-blue-800",
        "Cancelled" => "bg-red-100 text-red-800",
        _ => "bg-gray-100 text-gray-800"
    };
}
