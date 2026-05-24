using System.ComponentModel.DataAnnotations;
namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Enums;

public enum UnitProductAuditStatus
{
    [Display(Name = "Encontrado en local")]
    Found = 10,

    [Display(Name = "No encontrado")]
    NotFound = 20,

    [Display(Name = "Enviado a bodega de pérdidas")]
    SendToWritteOffWarehouse = 30,

    [Display(Name = "Enviado a bodega de bajas")]
    EnviadoABajas = 40,

    [Display(Name = "Enviado a recuperaciones")]
    EnviadoARecuperaciones = 50,
    
    [Display(Name = "Producto en exceso")]
    ExcessProduct = 60,

    StatusMismatch = 70
}
