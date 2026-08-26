using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using System;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;

public record MovimientoDetailDto(
    int MovimientoId,
    // Identificadores
    int ProductoVarianteId,
    int? UnidadProductoId,
    int BodegaId,
    // Tipo
    TipoMovimiento TipoMovimiento,
    // Cantidades y costos
    int Cantidad,
    decimal CostoUnitario,
    decimal CostoTotal,
    // Trazabilidad y referencias
    int? ReferenciaId,
    string? ReferenciaTipo,
    string? Lote,
    DateTime? FechaVencimiento,
    // Motivo y observaciones
    string? Motivo,
    string? Observaciones,
    // Auditoría
    DateTime CreatedAt,
    string CreatedBy
);