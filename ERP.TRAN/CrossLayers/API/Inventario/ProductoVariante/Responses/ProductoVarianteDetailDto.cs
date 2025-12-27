namespace ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Responses;

public sealed record ProductoVarianteDetailDto
(
    int? Id,
    string CodigoVariante,
    string? Atributos,
    decimal? PrecioVenta,
    decimal? CostoUnitario,
    int Stock,
    int StockMinimo,
    string? CodigoBarras,
    bool Activo
);
