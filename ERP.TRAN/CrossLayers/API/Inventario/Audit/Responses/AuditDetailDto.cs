using ERP.TRAN.CrossLayers.API.Inventario.Auditorias.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Categoria.Responses;

namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Responses;

public sealed record AuditDetailDto(
    int Id,
    DateTime StartDate,
    DateTime? EndDate,
    int? WarehouseId,
    string? WarehouseName,
    List<CategoriaDetailDto>  Categorias,
    int? ProductId,
    string? ProductName,
    string TypeDisplay,
    string StatusDisplay,
    int ResponsibleId,
    int? SupervisorId,
    int TotalExpectedUnits,
    int TotalCountedUnits,
    int TotalMatches,
    int TotalMissing,
    int TotalSurplus,
    int TotalLocationDifferences,
    int TotalStatusDifferences,
    string? Observations,
    string? Conclusions,
    DateTime CreatedAt,
    string CreatedBy
);
/// <summary>
/// Registra una o varias unidades encontradas físicamente (escaneo individual o bulk).
/// </summary>

/// <summary>
/// Cierra la auditoría y registra las conclusiones.
/// </summary>
public class CloseAuditRequest
{
    public int AuditId { get; set; }

    /// <summary>Conclusiones y observaciones finales del supervisor.</summary>
    public string? Conclusions { get; set; }

    /// <summary>Auth0 ID de quien cierra la auditoría.</summary>
    public int _CloserAuth0Id { get; set; }
}


// ═══════════════════════════════════════════════════════════════════════════════
// RESPONSES / DTOs
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>Resultado del procesamiento de RegisterFoundUnits.</summary>
public record RegisterFoundUnitResultDto(
    int AuditId,
    int TotalProcessed,
    int Successful,
    int Failed,
    IReadOnlyList<FoundUnitResultItemDto> Items);

/// <summary>Resultado individual por serial procesado.</summary>
public record FoundUnitResultItemDto(
    string Code,
    bool Success,
    string Message);

/// <summary>Detalle de una unidad registrada como sobrante.</summary>
public record SurplusUnitDto(
    int Id,
    int AuditId,
    string Serial,
    int? ProductoId,
    int? ProductoVarianteId,
    int? PhysicalWarehouseId,
    DateTime? CountedAt,
    string CountedBy);

/// <summary>
/// Progreso en tiempo real de la auditoría.
/// </summary>
public record AuditProgressDto(
    int AuditId,
    int? WarehouseId,
    string? WarehouseName,
    string Status,
    DateTime StartDate,
    int TotalExpected,
    int TotalCounted,
    int TotalFound,
    int TotalNotFound,
    int TotalSurplus,
    int LocationDiffs,
    int StatusDiffs,
    double CompletionPercent);