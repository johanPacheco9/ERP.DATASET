namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
public record BaseProductDto
(
    int Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    int CategoriaId,
    int? ProveedorId,
    string UnidadMedida,
    string? ImagenUrl,
    string? Tags,
    bool Activo
);

