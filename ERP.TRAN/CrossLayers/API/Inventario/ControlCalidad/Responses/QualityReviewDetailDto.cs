namespace ERP.TRAN.CrossLayers.API.Inventario.ControlCalidad.Responses;

public record QualityReviewDetailDto(
    int Id,
    int OrdenCompraId,
    int RecepcionId,
    string Status,
    string StatusDisplay,
    decimal TotalRecibido,
    decimal TotalAprobado,
    decimal TotalRechazado,
    string? ObservacionesGenerales,
    List<QualityReviewItemDto> Items
);

public record QualityReviewItemDto(
    int Id,
    int ProductoVarianteId,
    string? ProductoNombre,
    string? SKU,
    decimal CantidadRecibida,
    decimal CantidadAprobada,
    decimal CantidadRechazada,
    string? MotivoRechazo
);
