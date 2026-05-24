namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;

public record UnitProductAuditSummaryDto(
    int Id,
    int AuditId,
    int UnitProductId,
    int ProductoId,
    int ProductoVarianteId,
    int BodegaId,
    string Serial,
    string StatusDisplay,
    string? BodegaName,
    string? ProductoName,
    string? Observaciones,
    DateTime CreatedAt,
    // NUEVOS CAMPOS AGREGADOS AL RECORD PARA CONTROL VISUAL
    string? UbicacionFisica = null,
    string? EstadoFisico = null,
    string? MotivoDiferencia = null,
    bool RequiereAccionCorrectiva = false
);

