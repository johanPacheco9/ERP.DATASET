using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Responses;

public record UnitProductDetailDto(
    int Id,
    string Serial,
    ProductoStatus Status,
    DateTime? FechaVencimiento,
    string productName,
    string? ProductoImagenUrl,
    string? ProductoCodigo,
    string? CodigoVariante,
    string? Atributos, 
    decimal? PrecioVenta,
    string BodegaNombre
);