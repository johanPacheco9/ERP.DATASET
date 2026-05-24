namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Responses;

public record WarehouseDetailDTO
(
    int Id,
    string Nombre,
    string? Descripcion,
    string? Ubicacion,
    bool Activa,
    DateTime FechaCreacion,
    DateTime? FechaModificacion,
    decimal? Max_Capacity
);

