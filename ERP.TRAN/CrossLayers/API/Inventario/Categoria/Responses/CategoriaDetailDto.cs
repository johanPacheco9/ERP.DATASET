namespace ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;

public record CategoriaDetailDto
(
    int Id,
    string Nombre,
    string? Descripcion, 
    DateTime? FechaCreacion, 
    DateTime? FechaModificacion 
);

