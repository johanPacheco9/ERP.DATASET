namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Responses;

public record ProductoSummaryDto
(
    Guid? Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    decimal PrecioVenta, 
    decimal CostoUnitario,        
    string UnidadMedida,
    bool EsPerecedero,
    string? CategoriaNombre,
    string? ProveedorNombre,
    string? ImagenUrl,
    string? Tags,
    bool Activo
);
