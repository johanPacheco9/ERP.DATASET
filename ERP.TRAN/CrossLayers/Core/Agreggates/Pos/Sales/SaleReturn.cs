using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

/// <summary>
/// Historial de una anulación o devolución parcial sobre una venta.
/// Mantiene trazabilidad (EntityWithTraceability) y solo los identificadores
/// mínimos: ClienteId y ProductoVarianteId (o serial vía UnidadProductoId).
/// </summary>
public class SaleReturn : EntityWithtraceability
{
    public int Id { get; set; }

    /// <summary>Venta original afectada.</summary>
    public int SaleId { get; set; }
    public Sale? Sale { get; set; }

    /// <summary>Línea de venta original (null si es anulación total sin línea específica).</summary>
    public int? SaleLineItemId { get; set; }

    /// <summary>Cliente de la venta original.</summary>
    public int ClientId { get; set; }

    /// <summary>Variante de producto devuelta/anulada.</summary>
    public int ProductoVarianteId { get; set; }

    /// <summary>Unidad física (serial) si aplica; null para productos no serializados.</summary>
    public int? UnidadProductoId { get; set; }

    /// <summary>Cantidad devuelta/anulada.</summary>
    public int Quantity { get; set; }

    /// <summary>Motivo de la anulación/devolución.</summary>
    public string Motivo { get; set; } = null!;

    /// <summary>Indica si es anulación total de la venta.</summary>
    public bool IsFullCancellation { get; set; }
}
