using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;

public enum DniType
{
    [Display(Name = "Cédula de Ciudadanía")]
    cc = 10,

    [Display(Name = "Cédula de Extranjería")]
    ce = 20,

    [Display(Name = "NIT (Número de Identificación Tributaria)")]
    nit = 30,

    [Display(Name = "Tarjeta de Identidad")]
    ti = 40,

    [Display(Name = "Pasaporte")]
    pasaporte = 50,

    [Display(Name = "Registro Civil")]
    rc = 60,

    [Display(Name = "Permiso Especial de Permanencia (PEP)")]
    pep = 70
}
