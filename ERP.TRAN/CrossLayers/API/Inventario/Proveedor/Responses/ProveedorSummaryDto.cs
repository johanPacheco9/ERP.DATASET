namespace ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Responses;
public record ProveedorSummaryDto
    (
        Guid? Id,
        string Codigo,
        string Nombre,
        string? Descripcion,
        decimal PrecioVenta,
        string? ImagenUrl,
        string? Tags,
        bool Activo
    );

