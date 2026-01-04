
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;

public enum DniType
{
    [Display(Name ="Cédula Ciudadanía")]
    cc = 10,

    [Display(Name = "Cédula Extranjería")]
    ce = 20,
}
