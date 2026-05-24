namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;

public sealed record AuditSummaryDto(
    int Id,
    DateTime StartDate,
    DateTime? EndDate,
    string? WarehouseName,
    string? TypeDisplay,
    string? StatusDisplay,
    int TotalExpectedUnits,
    int TotalCountedUnits,
    int TotalMatches,
    int TotalMissing,
    DateTime CreatedAt
);
