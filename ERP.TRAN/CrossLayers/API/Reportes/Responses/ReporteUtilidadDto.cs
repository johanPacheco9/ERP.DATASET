namespace ERP.TRAN.CrossLayers.API.Reportes.Responses;

public sealed record ReporteUtilidadDto(
    decimal TotalIngresos,
    decimal TotalCostoVentas,
    decimal UtilidadBruta,
    decimal MargenBrutoPorcentaje,
    decimal TotalImpuestos,
    int CantidadVentas,
    int TotalUnidadesVendidas,
    IReadOnlyList<ItemUtilidadProductoDto> DesglosePorProducto
);

public sealed record ItemUtilidadProductoDto(
    int ProductoBaseId,
    int ProductoVarianteId,
    string NombreProducto,
    string Sku,
    int CantidadVendida,
    decimal IngresoTotal,
    decimal CostoTotal,
    decimal UtilidadBruta,
    decimal MargenPorcentaje
);
