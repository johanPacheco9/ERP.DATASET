namespace ERP.TRAN.CrossLayers.API.Reportes.Responses;

public sealed record ReporteProductosMasVendidosDto(
    int TotalProductosListados,
    int TotalUnidadesVendidas,
    decimal TotalVendido,
    IReadOnlyList<ProductoMasVendidoItemDto> Productos
);

public sealed record ProductoMasVendidoItemDto(
    int Ranking,
    int ProductoBaseId,
    int ProductoVarianteId,
    string NombreProducto,
    string Sku,
    int CantidadVendida,
    decimal TotalVendido,
    decimal CostoTotal,
    decimal UtilidadGenerada,
    decimal MargenPorcentaje
);
