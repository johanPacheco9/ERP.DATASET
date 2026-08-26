using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;

public enum ProductoVarianteStatus
{
    [Display(Name = "Inactivo")]
    Inactivo = 0,

    [Display(Name = "Activo")]
    Active = 1,

    [Display(Name = "Descontinuado")]
    Descontinuado = 2
}