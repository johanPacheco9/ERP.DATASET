using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Shifts.Enums;

public enum PosShiftStatus
{
    [Display(Name = "Abierto")]
    Open = 1,

    [Display(Name = "Cerrado")]
    Closed = 2
}
