using ERP.TRAN.CrossLayers.API.Inventario.ControlCalidad.Requests;
using ERP.TRAN.CrossLayers.API.Inventario.ControlCalidad.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Responses;
using Microsoft.AspNetCore.Components;

namespace ERP.DATASET.Components.Pages.Inventario.Compras;

public partial class OrdenCompraDetalle
{
    [Parameter] public int Id { get; set; }

    private OrdenCompraDetailDto? Orden;
    private RecepcionDetailDto? Recepcion;
    private QualityReviewDetailDto? Calidad;
    private bool Cargando = true;

    private string? MensajeError;

    protected override async Task OnInitializedAsync()
    {
        await CargarDatos();
    }

    private async Task CargarDatos()
    {
        Cargando = true;
        Orden = await OrdenesDeCompraManager.GetById(Id, default);
        
        if (Orden != null)
        {
            if (Orden.RecepcionId.HasValue)
                Recepcion = await RecepcionManager.GetById(Orden.RecepcionId.Value, default);

            if (Orden.QualityReviewId.HasValue)
                Calidad = await CalidadManager.GetByOrdenCompraId(Id, default);
        }
        Cargando = false;
    }

    private async Task AprobarCalidad()
    {
        if (Calidad == null) return;
        var req = new AprobarQualityReviewRequest { QualityReviewId = Calidad.Id, BodegaId = Recepcion?.BodegaId ?? 0 };
        var res = await CalidadManager.Aprobar(req, 1, default);
        if (res.IsSuccess) await CargarDatos(); else MensajeError = res.Error.Message;
    }

    private async Task AprobarOrden()
    {
        var result = await OrdenesDeCompraManager.Aprobar(Id, 1, cancellationToken: default);
        if (result.IsSuccess) await CargarDatos(); else MensajeError = result.Error.Message;
    }

    private async Task EnviarOrden()
    {
        var result = await OrdenesDeCompraManager.Enviar(Id, 1, cancellationToken: default);
        if (result.IsSuccess) await CargarDatos(); else MensajeError = result.Error.Message;
    }

    private async Task CancelarOrden()
    {
        var result = await OrdenesDeCompraManager.Cancelar(Id, 1, "Cancelada desde la vista de orden.", default);
        if (result.IsSuccess) await CargarDatos(); else MensajeError = result.Error.Message;
    }

    private async Task RechazarCalidad()
    {
        if (Calidad == null) return;
        var req = new RechazarQualityReviewRequest { QualityReviewId = Calidad.Id, MotivoRechazo = "Rechazo total desde UI" };
        var res = await CalidadManager.Rechazar(req, 1, default);
        if (res.IsSuccess) await CargarDatos(); else MensajeError = res.Error.Message;
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
