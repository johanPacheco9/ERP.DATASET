using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;

public enum TipoDocumentoElectronico
{
    Factura = 1,
    NotaCredito = 2,
    NotaDebito = 3,
    DocumentoSoporte = 4
}

public enum EstadoDocumentoElectronico
{
    Pendiente = 1,
    Procesando = 2,
    Validado = 3,
    ConErrores = 4
}

/// <summary>
/// Documento electrónico (Factus) asociado a una venta. Sirve también como cola
/// de emisión para el BackgroundService.
/// </summary>
public class DocumentoElectronico : EntityWithtraceability
{
    public int VentaId { get; set; }
    public Sale? Venta { get; set; }

    public TipoDocumentoElectronico Tipo { get; set; } = TipoDocumentoElectronico.Factura;

    /// <summary>Código de referencia único (SaleNumber).</summary>
    public string ReferenceCode { get; set; } = null!;

    public EstadoDocumentoElectronico Estado { get; set; } = EstadoDocumentoElectronico.Pendiente;

    public string? Numero { get; set; }
    public string? Cufe { get; set; }
    public bool IsValidated { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public string? ErroresDian { get; set; }
    public string? PublicUrl { get; set; }
    public string? QrUrl { get; set; }

    public int Intentos { get; set; }
    public DateTime? ProximoIntentoEn { get; set; }
    public string? UltimoError { get; set; }

    /// <summary>Token de concurrencia optimista para reclamar pendientes sin doble procesamiento.</summary>
    public byte[] RowVersion { get; set; } = null!;
}
