namespace ERP.TRAN.CrossLayers.API.Inventario.ProductoBase.Responses;
public record BaseProductDto
(
    int Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    List<CategoryDto> Categorias,
    int? ProveedorId,
    string UnidadMedida,
    string? ImagenUrl,
    string? Tags,
    bool Activo
);