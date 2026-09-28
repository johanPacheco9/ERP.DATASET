namespace ERP.TRAN.CrossLayers.API.Reportes.Responses;

public sealed record ReporteRotacionInventarioDto(
    DateTime Desde,
    DateTime Hasta,
    int TotalDiasPeriodo,
    int TotalItems,
    int TotalUnidadesEnStock,
    decimal TotalValorInventarioFifo,
    decimal TotalCostoVendidoPeriodo,
    decimal IndiceRotacionGlobal,
    decimal DiasInventarioGlobal,
    int CantidadAltaRotacion,
    int CantidadMediaRotacion,
    int CantidadBajaRotacion,
    int CantidadSinMovimiento,
    IReadOnlyList<ItemRotacionInventarioDto> Items,
    IReadOnlyList<CapaFifoDisponibleDto> CapasFifo,
    IReadOnlyList<ValorizacionPorBodegaDto> ValorizacionPorBodega,
    IReadOnlyList<ValorizacionPorCategoriaDto> ValorizacionPorCategoria
);

public sealed record ItemRotacionInventarioDto(
    int ProductoVarianteId,
    string Sku,
    string? CodigoBarras,
    string NombreProducto,
    string? Categoria,
    int BodegaId,
    string NombreBodega,
    int StockActual,
    int StockReservado,
    int StockMinimo,
    decimal CostoUnitarioFifoPromedio,
    decimal ValorTotalStockFifo,
    int UnidadesVendidasPeriodo,
    decimal CostoVentasPeriodo,
    decimal TotalIngresosPeriodo,
    decimal IndiceRotacion,
    decimal DiasInventario,
    string ClasificacionRotacion,
    DateTime? UltimaFechaEntrada,
    DateTime? UltimaFechaVenta
);

public sealed record CapaFifoDisponibleDto(
    int MovementId,
    int ProductoVarianteId,
    string Sku,
    string NombreProducto,
    int BodegaId,
    string NombreBodega,
    DateTime FechaEntrada,
    int CantidadOriginal,
    int CantidadRemanente,
    decimal CostoUnitario,
    decimal ValorRemanenteTotal,
    string? Lote,
    DateTime? FechaVencimiento
);

public sealed record ValorizacionPorBodegaDto(
    int BodegaId,
    string NombreBodega,
    int TotalUnidades,
    decimal TotalValorFifo,
    int VariantesDistintas
);

public sealed record ValorizacionPorCategoriaDto(
    int CategoriaId,
    string CategoriaNombre,
    int TotalUnidades,
    decimal TotalValorFifo
);
