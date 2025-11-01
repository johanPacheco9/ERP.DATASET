namespace ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Responses;
public record ProveedorDetailDto
    (
    Guid? Id,
    string Nombre,
    string? Nit,
    string? Direccion,
    DateTime? FechaCreacion,
    DateTime? FechaActualizacion,
    string? Telefono,
    bool Activo = true
    );

