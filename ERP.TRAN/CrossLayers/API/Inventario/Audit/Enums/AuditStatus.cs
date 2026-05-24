using System.ComponentModel.DataAnnotations;

public enum AuditStatus
{
    [Display(Name = "Pendiente de revisión")]
    Pendiente = 100,

    [Display(Name = "Completada")]
    Completada = 200,

    [Display(Name = "Rechazada por inconsistencias")]
    RejectWithinconsistences = 300,

    [Display(Name = "En progreso")]
    InProgress = 350,

}
