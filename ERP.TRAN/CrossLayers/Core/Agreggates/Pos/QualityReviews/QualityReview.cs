using ERP.TRAN.CrossLayers.API.QualityReviews.Responses;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.QualityReviews;

/// <summary>
/// Control de calidad aplicado a una recepción de compra antes de impactar inventario.
/// </summary>
public class QualityReview : EntityWithtraceability
{
    public int Id { get; set; }

    public int OrdenCompraId { get; set; }
    public OrdenCompra OrdenCompra { get; set; } = null!;

    public QualityReviewStatus Status { get; set; } = QualityReviewStatus.Pendiente;

    public string? ObservacionesGenerales { get; set; }

    /// <summary>
    /// Detalles ítem por ítem para verificar qué cantidad se aprueba y qué se rechaza.
    /// </summary>
    public ICollection<QualityReviewDetail> Detalles { get; set; } = new List<QualityReviewDetail>();
}