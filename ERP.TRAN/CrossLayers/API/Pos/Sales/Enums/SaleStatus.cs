using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Enums;

public enum SaleStatus
{
    [Display(Name = "Completada")]
    Completed = 10,

    [Display(Name = "Anulada")]
    Cancelled = 20,

    [Display(Name = "Pendiente")]
    Pending = 30,

    [Display(Name = "Falló")]
    Failed = 40
    
}
