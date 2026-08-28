using ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Enums;
using System;
using System.Collections.Generic;

namespace ERP.TRAN.CrossLayers.API.Inventario.Movimientos.Responses;

public record MovimientoItemDto(
    int UnidadProductoId,
    int ProductoVarianteId,
    string? SerialNumber,
    string? Lote,
    DateTime? FechaVencimiento
);

public record MovimientoDetailDto(
    int MovimientoId,
    // Bodegas (Origen y Destino)
    int? BodegaOrigenId,
    string? NombreBodegaOrigen,
    int? BodegaDestinoId,
    string? NombreBodegaDestino,
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
    // Items / Detalles
    List<MovimientoItemDto> Items,
    // Auditoría
    DateTime CreatedAt,
    string CreatedBy
);