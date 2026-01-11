namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;

public record UnitProductDetailDto(
    int Id,
    string Serial,
    string Estado,
    DateTime FechaIngreso,

    // Información esencial del producto
    string productName,
    string? ProductoImagenUrl,
    string? ProductoCodigo,

    // Información de la variante
    string? VarianteNombre,
    string? Atributos,  // "Rojo, Talla M"
    decimal? PrecioVenta,
    // Ubicación actual
    string BodegaNombre // "Estante A-5"
);