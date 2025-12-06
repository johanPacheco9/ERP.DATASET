using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;

public enum ProductoAuditadoStatus
{
    [Display(Name = "Encontrado en local")]
    Encontrado = 10,

    [Display(Name = "No encontrado")]
    NoEncontrado = 20,

    [Display(Name = "Enviado a bodega de pérdidas")]
    EnviadoAPerdidas = 30,

    [Display(Name = "Enviado a bodega de bajas")]
    EnviadoABajas = 40,

    [Display(Name = "Enviado a recuperaciones")]
    EnviadoARecuperaciones = 50

}
