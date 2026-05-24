using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;

public enum ProductoEnumStatus
{
    [Display(Name = "Inactivo")]
    Inactivo = 0,

    [Display(Name = "IsActive para venta")]
    Activo = 1,

    [Display(Name = "Descontinuado")]
    Descontinuado = 2,

    [Display(Name = "En Desarrollo")]
    EnDesarrollo = 3,

    [Display(Name = "Bloqueado")]
    Bloqueado = 4,

    [Display(Name = "Agotado")]
    Agotado = 5
}
