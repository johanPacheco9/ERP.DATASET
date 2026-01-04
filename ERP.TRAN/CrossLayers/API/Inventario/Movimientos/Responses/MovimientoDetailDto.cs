using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using System;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;

public record MovimientoDetailDto(
    int MovimientoId,
    // Identificadores
    int ProductoId,
    int ProductoVarianteId,
    int BodegaId,
    // Tipo
    TipoMovimiento TipoMovimiento,
    // Cantidades y costos
    int Cantidad,
    decimal CostoUnitario,
    decimal CostoTotal,
    // Lote / vencimiento
    string? Lote,
    string? Referencia,
    // Motivo y observaciones
    string? Motivo,
    string? Observaciones,
    // Auditoría
    DateTime CreatedAt,
    string CreatedBy
);