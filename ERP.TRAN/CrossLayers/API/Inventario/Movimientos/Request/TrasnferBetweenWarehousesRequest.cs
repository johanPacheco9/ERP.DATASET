
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public sealed class TrasnferBetweenWarehousesRequest
{
    public List<int> ProductIds { get; set; } = [];

    public int OriginWarehoseId { get; set; }
    
    public int DestinationWarehoseId { get; set; }
    
    [Display(Name ="")]
    
    public string? Observations { get; set;  }
}
