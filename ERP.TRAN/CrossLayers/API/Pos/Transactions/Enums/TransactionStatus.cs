
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Pos.Transactions.Enums;

public enum TransactionStatus
{

    [Display(Name = "Apartada")]
    Separated = 10,

    [Display(Name = "Anulada")]
    Canceled = 20,

    [Display(Name = "Entregada al cliente")]
    Entregada = 30,

    [Display(Name = "Bloqueada por auditoría")]
    LossWarehouse = 40,
}