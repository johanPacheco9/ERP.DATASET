using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Base.ResultPattern;

public enum ErrorType
{
    [Display(Name = "Sin error")]
    None = 0,
    
    [Display(Name = "No encontrado")]
    NotFound = 1,
    
    [Display(Name = "Error de validación")]
    Validation = 2,
    
    [Display(Name = "Conflicto")]
    Conflict = 3,
    
    [Display(Name = "Error interno")]
    Failure = 4,
    
    [Display(Name = "No autorizado")]
    Unauthorized = 5
}