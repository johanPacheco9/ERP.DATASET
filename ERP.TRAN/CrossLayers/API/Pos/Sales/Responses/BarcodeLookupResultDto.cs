using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;

namespace ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;

public sealed record BarcodeLookupResultDto(
    int ProductoBaseId,
    int? ProductoVarianteId,
    string Name,
    string Code,
    string? SKU,
    string? CodigoBarras,
    string? Serial,
    decimal PrecioVenta,
    decimal PorcentajeIVA,
    bool ExentoIVA,
    int AvailableStock,
    string? ImagenUrl,
    string UnidadMedida,
    List<CategoriaDetailDto>? Categorias
);
