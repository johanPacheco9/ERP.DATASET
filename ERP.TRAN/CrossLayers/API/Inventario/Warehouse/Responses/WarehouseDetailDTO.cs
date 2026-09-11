using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;

namespace ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Responses;

public record WarehouseDetailDTO(
    int Id,
    string Nombre,
    string? Descripcion,
    string? Ubicacion,
    bool Activa,
    DateTime FechaCreacion,
    DateTime? FechaModificacion,
    decimal? Max_Capacity,
    string? Code,
    WarehouseType Type
);
