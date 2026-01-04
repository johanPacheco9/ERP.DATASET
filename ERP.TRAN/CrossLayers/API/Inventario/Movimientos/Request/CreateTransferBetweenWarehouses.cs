
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public sealed class CreateTransferBetweenWarehouses
{
    [Display(Name ="")]
    public string Motivo { get; set;  }
}
