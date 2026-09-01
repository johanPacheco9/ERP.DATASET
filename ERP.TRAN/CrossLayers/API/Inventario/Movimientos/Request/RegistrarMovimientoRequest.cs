using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Request;

public sealed class RegistrarMovimientoRequest
{
    public List<int> ProductIds { get; set; } = [];

    public int OriginWarehouseId { get; set; } // Bodega principal (Origen o donde ocurre la entrada/salida/baja)
    
    public int? DestinationWarehouseId { get; set; } // Solo requerido si es Transferencia
    
    /// <summary>
    /// Opcional, se puede obtener una bodega por el originwarehouseId.
    /// en este caso se usará para obtener la bodega de perdidas o bajas de una tienda.
    /// </summary>
    public int? StoreId { get; set; }
    
    public TipoMovimiento TipoMovimiento { get; set; }

    [Display(Name = "Observaciones")]
    public string? Observations { get; set; }
}