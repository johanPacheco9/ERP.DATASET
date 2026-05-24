using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;

public enum AuditType
{
    [Display(Name = "Auditoría General")]
    General = 10,

    [Display(Name = "Auditoría Cíclica")]
    Cyclical = 20,

    [Display(Name = "Auditoría Selectiva")]
    Selective = 30,

    [Display(Name = "Auditoría Sorpresa")]
    Surprise = 40,

    [Display(Name = "Post-Movement")]
    PostMovement = 50,

    [Display(Name = "Auditoría Mensual")]
    Monthly = 60,

    [Display(Name = "Auditoría Anual")]
    Annual = 70
}

