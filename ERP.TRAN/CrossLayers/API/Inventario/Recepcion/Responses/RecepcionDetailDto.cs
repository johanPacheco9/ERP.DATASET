namespace ERP.TRAN.CrossLayers.API.Inventario.Recepcion.Responses;

public record RecepcionDetailDto(
    int Id,
    int OrdenCompraId,
    int BodegaId,
    string BodegaNombre,
    string Status,
    string StatusDisplay,
    DateTime FechaRecepcion,
    string? GuiaRemisionProveedor,
    string? Observaciones,
    List<DetalleRecepcionDto> Detalles
);

public record DetalleRecepcionDto(
    int Id,
    int ProductoVarianteId,
    string? ProductoNombre,
    string? SKU,
    decimal CantidadEsperada,
    decimal CantidadRecibida,
    string? ObservacionItem
);
