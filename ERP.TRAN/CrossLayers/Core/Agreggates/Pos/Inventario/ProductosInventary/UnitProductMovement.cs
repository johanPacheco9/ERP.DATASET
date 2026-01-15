
using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;

/// <summary>
/// Clase para el registro de movimientos por unidad de producto
/// </summary>
public class UnitProductMovement
{
    public int Id { get; set; }
    public int ProductoUnidadId { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }

    public int BodegaOrigenId { get; set; }
    public int? BodegaDestinoId { get; set; }

    public string Motivo { get; set; } = null!;
    public string? Observaciones { get; set; }

    public UnitProduct ProductoUnidad { get; set; } = null!;
}
