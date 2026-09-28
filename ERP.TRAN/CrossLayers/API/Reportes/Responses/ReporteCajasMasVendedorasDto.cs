namespace ERP.TRAN.CrossLayers.API.Reportes.Responses;

public sealed record ReporteCajasMasVendedorasDto(
    decimal GranTotalVendido,
    int TotalTransacciones,
    IReadOnlyList<CajaMasVendedoraItemDto> Cajas
);

public sealed record CajaMasVendedoraItemDto(
    int PosTerminalId,
    string TerminalNombre,
    string TerminalCode,
    string StoreNombre,
    int TotalTransacciones,
    decimal TotalVendido,
    decimal TotalEfectivo,
    decimal TotalTarjeta,
    decimal TotalTransferencia,
    decimal TotalCredito,
    decimal TicketPromedio
);
