using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Compras;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.QualityReviews;


/// <summary>
/// Detalle del control de calidad por cada producto/variante de la orden.
/// </summary>
public class QualityReviewDetail
{
    public int Id { get; set; }
    
    public int QualityReviewId { get; set; }
    public QualityReview QualityReview { get; set; } = null!;

    public int DetalleOrdenCompraId { get; set; }
    public DetalleOrdenCompra DetalleOrdenCompra { get; set; } = null!;

    public decimal CantidadRecibida { get; set; }  // Lo que físicamente llegó del proveedor
    public decimal CantidadAprobada { get; set; }  // Lo que pasa el filtro de calidad
    public decimal CantidadRechazada { get; set; } // Lo que viene defectuoso o no cumple

    public string? MotivoRechazo { get; set; }     // Ej: "Empaque roto", "Vencimiento próximo", "No cumple especificaciones"
}