namespace ERP.TRAN.CrossLayers.API.Inventario.OrdenDeCompra.Responses;

public record OrdenCompraDetailDto(
    int Id,
    int ProveedorId,
    string ProveedorNombre,
    string Status,
    string StatusDisplay,
    DateTime FechaCompra,
    decimal Subtotal,
    decimal Impuestos,
    decimal Total,
    int? RecepcionId,
    int? QualityReviewId,
    List<DetalleOrdenCompraDto> Detalles,
    List<OrdenCompraComentarioDto> Observaciones
);

public record DetalleOrdenCompraDto(
    int Id,
    int ProductoVarianteId,
    string? ProductoNombre,
    string? SKU,
    decimal Cantidad,
    decimal CostoUnitario,
    decimal Descuento,
    decimal Impuesto,
    decimal Total
);

public record OrdenCompraComentarioDto(
    int Id,
    string Texto,
    DateTime Fecha,
    string EstadoAsociado
);

public record OrdenCompraSummaryDto(
    int Id,
    string ProveedorNombre,
    string Status,
    string StatusDisplay,
    DateTime FechaCompra,
    decimal Total,
    int TotalItems,
    bool TieneRecepcion,
    bool TieneControlCalidad
);
