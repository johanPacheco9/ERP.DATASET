using System.ComponentModel.DataAnnotations;

public enum AuditoriaStatus
{
    [Display(Name = "Pendiente de revisión")]
    Pendiente = 100,

    [Display(Name = "Completada")]
    Completada = 200,

    [Display(Name = "Rechazada por inconsistencias")]
    RechazadoConInconsistencias = 300
}
