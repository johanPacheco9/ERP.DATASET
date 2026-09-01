using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;

namespace ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Responses;

public record UnidadProductoDetailDto(
    int Id,
    string Serial,
    UnidadProductoStatus  Status,
    DateTime? FechaVencimiento,
    string productName,
    string? ProductoImagenUrl,
    string? ProductoCodigo,
    string? CodigoVariante,
    string? Atributos, 
    decimal? PrecioVenta,
    string BodegaNombre
);