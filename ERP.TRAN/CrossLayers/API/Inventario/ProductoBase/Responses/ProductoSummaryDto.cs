using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Responses;

namespace ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Responses;

public record ProductoSummaryDto
(
    int? Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    decimal PrecioVenta,
    decimal CostoUnitario,
    string UnidadMedida,
    bool EsPerecedero,
    List<CategoryDto>  Categorias,
    string? MarcaNombre,
    List<string> Proveedores,
    string? ImagenUrl,
    string? Tags,
    bool Activo,
    List<ProductoVarianteDetailDto> ProductoVariantes
);