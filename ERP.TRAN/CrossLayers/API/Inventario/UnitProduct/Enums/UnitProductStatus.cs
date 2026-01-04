using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;

public enum UnitProductStatus
{
    [Display(Name = "Available")]
    Available = 10,

    [Display(Name = "Sold")]
    Sold = 20,

    [Display(Name = "Transferred")]
    Transferred = 30,

    [Display(Name = "Blocked")]
    Blocked = 40,

    [Display(Name = "Lost")]
    Lost = 50,

    [Display(Name = "Damaged")]
    Damaged = 60
}
