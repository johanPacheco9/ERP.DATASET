using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.QualityReviews.Responses;

public enum QualityReviewStatus
{
    [Display(Name = "Pendiente")]
    Pendiente = 0,
    
    [Display(Name = "Aprobado")]
    Aprobado = 1,
    
    [Display(Name = "Aprobado Parcialmente")]
    AprobadoParcial = 2,
    
    [Display(Name = "Rechazado")]
    Rechazado = 3
}