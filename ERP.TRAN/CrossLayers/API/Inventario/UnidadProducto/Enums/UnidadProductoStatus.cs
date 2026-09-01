using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;

/// <summary>
/// Enum que refleja los estados del ciclo de vida de una unidad de producto física.
/// </summary>
public enum UnidadProductoStatus
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

    [Display(Name = "Dañado / Dado de baja")]
    Damaged = 60,

    [Display(Name = "Separado / Apartado")]
    Separated = 70,
    
    [Display(Name = "Bloqueado por auditoría")]
    InAuditLock = 80,
    
    [Display(Name = "Recuperado")]
    Recovered = 90
}