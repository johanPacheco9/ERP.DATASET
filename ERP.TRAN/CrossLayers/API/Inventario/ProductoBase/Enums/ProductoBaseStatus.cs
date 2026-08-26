using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;

public enum ProductoBaseStatus
{
    [Display(Name = "Inactivo")]
    Inactivo = 0,

    [Display(Name = "Activo para venta")]
    Active = 1,

    [Display(Name = "Descontinuado")]
    Descontinuado = 2,

    [Display(Name = "En desarrollo")]
    EnDesarrollo = 3,

    [Display(Name = "Bloqueado")]
    Bloqueado = 4
}
