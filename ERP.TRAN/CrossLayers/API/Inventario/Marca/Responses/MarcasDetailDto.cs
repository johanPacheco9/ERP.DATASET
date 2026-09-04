namespace ERP.TRAN.CrossLayers.API.Inventario.Marca.Responses;

public sealed record MarcasDetailDto(
    int Id,
    string Nombre,
    string? Descripcion,
    string? LogoUrl,
    bool Activa,
    DateTime? FechaCreacion
);