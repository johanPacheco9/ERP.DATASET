namespace ERP.TRAN.CrossLayers.API.Inventario.ControlCalidad.Requests;

public class CreateQualityReviewRequest
{
    public int OrdenCompraId { get; set; }
    public int RecepcionId { get; set; }
    public string? ObservacionesGenerales { get; set; }
    public List<QualityReviewItemRequest> Items { get; set; } = new();
}

public class QualityReviewItemRequest
{
    public int DetalleOrdenCompraId { get; set; }
    public int ProductoVarianteId { get; set; }
    public decimal CantidadRecibida { get; set; }
    public decimal CantidadAprobada { get; set; }
    public decimal CantidadRechazada { get; set; }
    public string? MotivoRechazo { get; set; }
}

public class AprobarQualityReviewRequest
{
    public int QualityReviewId { get; set; }
    public int BodegaId { get; set; }
    public string? Observaciones { get; set; }
}

public class RechazarQualityReviewRequest
{
    public int QualityReviewId { get; set; }
    public string MotivoRechazo { get; set; } = string.Empty;
}
