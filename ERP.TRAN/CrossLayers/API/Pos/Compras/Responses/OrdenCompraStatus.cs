using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Compras.Responses;
/// <summary>
/// Estados del ciclo de vida de una orden de compra.
/// </summary>
public enum OrdenCompraStatus
{
    [Display(Name = "Borrador")]
    Draft = 10,

    [Display(Name = "Pendiente de aprobación")]
    PendingApproval = 20,

    [Display(Name = "Aprobada")]
    Approved = 30,

    [Display(Name = "Enviada al proveedor")]
    Sent = 40,

    [Display(Name = "Parcialmente recibida")]
    PartiallyReceived = 50,

    [Display(Name = "Recibida")]
    Received = 60,

    [Display(Name = "Cancelada")]
    Cancelled = 70,

    [Display(Name = "Finalizada")]
    Finalized = 80
}