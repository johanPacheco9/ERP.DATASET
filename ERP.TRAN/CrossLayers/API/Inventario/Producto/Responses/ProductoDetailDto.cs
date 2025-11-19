namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;
public record ProductoBaseDto
(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    Guid CategoriaId,
    Guid? ProveedorId,
    string UnidadMedida,
    string? ImagenUrl,
    string? Tags,
    bool Activo
);

