using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;

public enum ProductoStatus
{
    [Display(Name = "Disponible")]
    Available = 10,

    [Display(Name = "Vendido")]
    Sold = 20,

    [Display(Name = "Transferido")]
    Transferred = 30,

    [Display(Name = "Bloqueado")]
    Blocked = 40,

    [Display(Name = "Perdido")]
    Lost = 50,

    [Display(Name = "Dañado")]
    Damaged = 60,

    [Display(Name = "Separado")]
    Separated = 70,
    
    [Display(Name = "Bloqueado por auditoría")]
    InAuditLock = 80
}
