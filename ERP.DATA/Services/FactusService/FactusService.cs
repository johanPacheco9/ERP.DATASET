using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ERP.DATA.Services.FactusService;

/// <summary>
/// Servicio de integración con la API de Factus (facturación electrónica Colombia).
/// Mapea una <see cref="ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales.Sale"/> a la
/// estructura esperada por Factus y persiste la respuesta (CUFE, número, QR, estado).
/// </summary>
public sealed class FactusService
{
    private readonly HttpClient _httpClient;
    private readonly IFactusTokenProvider _tokenProvider;
    private readonly FactusRateLimiter _rateLimiter;
    private readonly FactusOptions _options;
    private readonly MainDataContext _context;
    private readonly ILogger<FactusService> _logger;

    public FactusService(HttpClient httpClient, IFactusTokenProvider tokenProvider, FactusRateLimiter rateLimiter, IOptions<FactusOptions> options, MainDataContext context, ILogger<FactusService> logger)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _rateLimiter = rateLimiter;
        _options = options.Value;
        _context = context;
        _logger = logger;
        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
    }

    public async Task<FactusEmissionResult> EmitirFacturaAsync(int saleId, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines).ThenInclude(l => l.ProductoVariante).ThenInclude(v => v.ProductoBase)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == saleId, cancellationToken);

        if (sale is null)
            return FactusEmissionResult.Failure("Venta no encontrada.");

        var doc = await _context.DocumentosElectronicos
            .FirstOrDefaultAsync(d => d.VentaId == saleId && d.Tipo == TipoDocumentoElectronico.Factura, cancellationToken);

        if (doc is not null && (doc.Estado == EstadoDocumentoElectronico.Validado))
            return FactusEmissionResult.Failure("La venta ya fue emitida a Factus.");

        if (doc is null)
        {
            doc = new DocumentoElectronico
            {
                VentaId = sale.Id,
                Tipo = TipoDocumentoElectronico.Factura,
                ReferenceCode = sale.SaleNumber,
                Estado = EstadoDocumentoElectronico.Pendiente,
                Intentos = 0,
                CreatedAt = DateTime.UtcNow
            };
            _context.DocumentosElectronicos.Add(doc);
            await _context.SaveChangesAsync(cancellationToken);
        }

        doc.Estado = EstadoDocumentoElectronico.Procesando;
        doc.Intentos++;
        await _context.SaveChangesAsync(cancellationToken);

        return await ProcesarDocumentoAsync(doc.Id, cancellationToken);


    }

    public async Task<FactusEmissionResult> ProcesarDocumentoAsync(int documentoId, CancellationToken cancellationToken = default)
    {
        var doc = await _context.DocumentosElectronicos.FirstOrDefaultAsync(d => d.Id == documentoId, cancellationToken);
        if (doc is null)
            return FactusEmissionResult.Failure("Documento no encontrado.");

        var sale = await _context.Sales
            .Include(s => s.Client)
            .Include(s => s.Warehouse)
            .Include(s => s.Lines).ThenInclude(l => l.ProductoVariante).ThenInclude(v => v.ProductoBase)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == doc.VentaId, cancellationToken);

        if (sale is null)
            return FactusEmissionResult.Failure("Venta no encontrada.");

        var request = BuildRequest(sale);

        for (var attempt = 0; attempt < 2; attempt++)
        {
            await _rateLimiter.WaitAsync(cancellationToken);

            var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v2/bills/validate")
            {
                Content = JsonContent.Create(request, options: FactusJson.Options)
            };
            httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                await RegistrarErrorAsync(doc, ex.Message, recuperable: true, cancellationToken);
                return FactusEmissionResult.Failure(ex.Message);
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await ProcesarRespuestaExitosaAsync(sale, doc, body, cancellationToken);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _tokenProvider.InvalidateToken();
                continue;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var existente = await ConsultarPorReferenciaAsync(sale.SaleNumber, cancellationToken);
                var isValidated = existente?.IsValidated ?? false;

                if (!isValidated)
                {
                    await EliminarNoValidadaAsync(sale.SaleNumber, cancellationToken);
                    if (attempt == 0) continue;
                }
                else
                {
                    await ActualizarDesdeExistenteAsync(sale, doc, existente!, cancellationToken);
                    return FactusEmissionResult.Success(doc.Numero, doc.Cufe, doc.QrUrl);
                }

                await RegistrarErrorAsync(doc, $"Conflicto 409: {body}", recuperable: false, cancellationToken);
                return FactusEmissionResult.Failure("Conflicto 409");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5);
                await Task.Delay(retryAfter, cancellationToken);
                if (attempt == 0) continue;
                await RegistrarErrorAsync(doc, $"429: {body}", recuperable: true, cancellationToken);
                return new FactusRateLimitException("Límite de tasa de Factus.", retryAfter).Message is var m2 ? FactusEmissionResult.Failure(m2) : FactusEmissionResult.Failure("429");
            }

            if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                await RegistrarErrorAsync(doc, $"Validación ({(int)response.StatusCode}): {body}", recuperable: false, cancellationToken);
                throw new FactusValidationException($"Error de validación Factus: {body}");
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt + 1)), cancellationToken);
            if (attempt == 1)
            {
                await RegistrarErrorAsync(doc, $"Servidor ({(int)response.StatusCode}): {body}", recuperable: true, cancellationToken);
                return new FactusServerException($"Error de servidor Factus: {body}", response.StatusCode).Message is var m3 ? FactusEmissionResult.Failure(m3) : FactusEmissionResult.Failure("5xx");
            }
        }

        return FactusEmissionResult.Failure("No se pudo emitir la factura.");
    }



    private async Task<HttpResponseMessage> SendWithResilienceAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Reintentos para 5xx ya manejados en el bucle; aquí solo enviamos.
        return await _httpClient.SendAsync(request, cancellationToken);
    }

    private async Task<FactusEmissionResult> ProcesarRespuestaExitosaAsync(
        ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales.Sale sale,
        DocumentoElectronico doc,
        string body,
        CancellationToken cancellationToken)
    {
        try
        {
            using var json = System.Text.Json.JsonDocument.Parse(body);
            var root = json.RootElement;
            var data = root.TryGetProperty("data", out var d) ? d : root;
            var bill = data.TryGetProperty("bill", out var b) ? b : data;

            var number = bill.TryGetProperty("number", out var n) ? n.GetString() : null;
            var cufe = bill.TryGetProperty("cufe", out var c) ? c.GetString() : null;
            var qr = bill.TryGetProperty("qr", out var q) ? q.GetString() : null;
            var publicUrl = bill.TryGetProperty("public_url", out var pu) ? pu.GetString() : null;
            var isValidated = bill.TryGetProperty("is_validated", out var iv) && iv.ValueKind == System.Text.Json.JsonValueKind.True;
            var validatedAtStr = bill.TryGetProperty("validated_at", out var va) ? va.GetString() : null;

            // Comparar total de Factus con total de la venta.
            if (data.TryGetProperty("totals", out var totals) && totals.TryGetProperty("total", out var totalEl))
            {
                var factusTotalStr = totalEl.GetString() ?? totalEl.ToString();
                if (decimal.TryParse(factusTotalStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var factusTotal))
                {
                    if (factusTotal != sale.Total)
                    {
                        _logger.LogWarning("Diferencia de totales Factus vs venta {SaleId}: Factus={FactusTotal} Venta={SaleTotal}", sale.Id, factusTotal, sale.Total);
                    }
                }
            }

            doc.Numero = number;
            doc.Cufe = cufe;
            doc.QrUrl = qr;
            doc.PublicUrl = publicUrl;
            doc.IsValidated = isValidated;
            doc.Estado = EstadoDocumentoElectronico.Validado;
            doc.ErroresDian = null;
            doc.UltimoError = null;
            if (!string.IsNullOrEmpty(validatedAtStr) &&
                DateTime.TryParseExact(validatedAtStr, "dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var va2))
            {
                doc.ValidatedAt = va2;
            }
            else
            {
                doc.ValidatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return FactusEmissionResult.Success(number, cufe, qr);
        }
        catch (Exception ex)
        {
            await RegistrarErrorAsync(doc, $"Parseo: {ex.Message}. Body: {body}", recuperable: true, cancellationToken);
            return FactusEmissionResult.Failure($"Respuesta de Factus no parseable: {ex.Message}");
        }
    }

    private async Task<FactusDocumentoExistente?> ConsultarPorReferenciaAsync(string referenceCode, CancellationToken cancellationToken)
    {
        // Re-enviar crear con el mismo reference_code devuelve la factura existente.
        try
        {
            await _rateLimiter.WaitAsync(cancellationToken);
            var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
            var req = new FactusBillRequest { ReferenceCode = referenceCode };
            using var http = new HttpRequestMessage(HttpMethod.Post, "/v2/bills/validate")
            {
                Content = JsonContent.Create(req, options: FactusJson.Options)
            };
            http.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var resp = await _httpClient.SendAsync(http, cancellationToken);
            var body = await resp.Content.ReadAsStringAsync(cancellationToken);
            if (!resp.IsSuccessStatusCode) return null;
            using var json = System.Text.Json.JsonDocument.Parse(body);
            var data = json.RootElement.TryGetProperty("data", out var d) ? d : json.RootElement;
            var bill = data.TryGetProperty("bill", out var b) ? b : data;
            return new FactusDocumentoExistente
            {
                IsValidated = bill.TryGetProperty("is_validated", out var iv) && iv.ValueKind == System.Text.Json.JsonValueKind.True,
                Number = bill.TryGetProperty("number", out var n) ? n.GetString() : null,
                Cufe = bill.TryGetProperty("cufe", out var c) ? c.GetString() : null,
                QrUrl = bill.TryGetProperty("qr", out var q) ? q.GetString() : null
            };
        }
        catch
        {
            return null;
        }
    }

    private async Task EliminarNoValidadaAsync(string referenceCode, CancellationToken cancellationToken)
    {
        await _rateLimiter.WaitAsync(cancellationToken);
        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        using var http = new HttpRequestMessage(HttpMethod.Delete, $"/v2/bills/destroy/reference/{Uri.EscapeDataString(referenceCode)}");
        http.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _httpClient.SendAsync(http, cancellationToken);
    }

    private async Task ActualizarDesdeExistenteAsync(
        ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales.Sale sale,
        DocumentoElectronico doc,
        FactusDocumentoExistente existente,
        CancellationToken cancellationToken)
    {
        doc.Numero = existente.Number;
        doc.Cufe = existente.Cufe;
        doc.QrUrl = existente.QrUrl;
        doc.IsValidated = existente.IsValidated;
        doc.Estado = existente.IsValidated ? EstadoDocumentoElectronico.Validado : EstadoDocumentoElectronico.ConErrores;
        doc.ValidatedAt = existente.IsValidated ? DateTime.UtcNow : doc.ValidatedAt;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task RegistrarErrorAsync(DocumentoElectronico doc, string error, bool recuperable, CancellationToken cancellationToken)
    {
        doc.UltimoError = error;
        doc.ErroresDian = error;
        doc.Estado = recuperable ? EstadoDocumentoElectronico.Pendiente : EstadoDocumentoElectronico.ConErrores;
        if (!recuperable) doc.ProximoIntentoEn = null;
        else doc.ProximoIntentoEn = DateTime.UtcNow.AddMinutes(Math.Min(60, Math.Pow(2, doc.Intentos)));
        await _context.SaveChangesAsync(cancellationToken);
    }
    private FactusBillRequest BuildRequest(ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales.Sale sale)
    {
        var client = sale.Client;
        var legalOrg = client.DniType == ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType.nit ? "1" : "2";

        var customer = new FactusCustomer
        {
            IdentificationDocumentCode = MapDniType(client.DniType),
            Identification = client.IdentificationNumber,
            Dv = client.Dv,
            LegalOrganizationCode = legalOrg,
            TributeCode = "ZZ",
            Company = legalOrg == "1" ? client.Name : null,
            Names = legalOrg == "2" ? client.Name : null,
            Address = client.Address,
            Email = client.Email,
            Phone = client.PhoneNumber,
            CountryCode = "CO",
            MunicipalityCode = null,
            Responsibilities = new[] { "R-99-PN" }
        };

        var items = sale.Lines.Select(l => new FactusItem
        {
            CodeReference = l.ProductoVariante?.SKU ?? l.ProductoVarianteId.ToString(),
            Name = l.ProductoVariante?.ProductoBase?.Name ?? "Producto",
            Quantity = l.Quantity.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
            Price = l.UnitPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
            UnitMeasureCode = "94",
            StandardCode = "999",
            Taxes = new List<FactusTax>
            {
                new FactusTax
                {
                    Code = "01",
                    Rate = (l.TaxRate * 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                }
            }
        }).ToList();

        var paymentDetails = sale.Payments.Select(p => new FactusPaymentDetail
        {
            PaymentForm = p.Method == ERP.TRAN.CrossLayers.API.Pos.Payments.Enums.PaymentMethod.Cash ? "1" : "2",
            PaymentMethodCode = MapPaymentMethod(p.Method),
            Amount = p.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
            ReferenceCode = null
        }).ToList();

        if (paymentDetails.Count == 0)
        {
            paymentDetails.Add(new FactusPaymentDetail
            {
                PaymentForm = "1",
                PaymentMethodCode = "10",
                Amount = sale.Total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
            });
        }

        return new FactusBillRequest
        {
            ReferenceCode = sale.SaleNumber,
            Document = "01",
            NumberingRangeId = _options.NumberingRangeId,
            OperationType = "10",
            Observation = sale.Notes,
            SendEmail = _options.SendEmail,
            Customer = customer,
            Items = items,
            PaymentDetails = paymentDetails
        };
    }

    private static string MapDniType(ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType dniType) => dniType switch
    {
        ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType.cc => "13",
        ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType.nit => "31",
        ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType.ce => "22",
        ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType.pasaporte => "41",
        _ => "13"
    };

    private static string MapPaymentMethod(ERP.TRAN.CrossLayers.API.Pos.Payments.Enums.PaymentMethod method) => method switch
    {
        ERP.TRAN.CrossLayers.API.Pos.Payments.Enums.PaymentMethod.Cash => "10",
        ERP.TRAN.CrossLayers.API.Pos.Payments.Enums.PaymentMethod.DebitCard => "42",
        ERP.TRAN.CrossLayers.API.Pos.Payments.Enums.PaymentMethod.CreditCard => "48",
        ERP.TRAN.CrossLayers.API.Pos.Payments.Enums.PaymentMethod.BankTransfer => "47",
        ERP.TRAN.CrossLayers.API.Pos.Payments.Enums.PaymentMethod.Nequi => "42",
        _ => "10"
    };

    public async Task<List<DocumentoElectronico>> ListDocumentosAsync(int saleId, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentosElectronicos
            .AsNoTracking()
            .Where(d => d.VentaId == saleId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

public async Task<FactusArchivoResult> DescargarPdfAsync(int documentoId, CancellationToken cancellationToken = default)
    {
        var doc = await _context.DocumentosElectronicos.FirstOrDefaultAsync(d => d.Id == documentoId, cancellationToken);
        if (doc is null || string.IsNullOrEmpty(doc.Numero))
            return FactusArchivoResult.Failure("Documento no encontrado o sin número de factura.");

        await _rateLimiter.WaitAsync(cancellationToken);
        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        using var http = new HttpRequestMessage(HttpMethod.Get, $"/v2/bills/{Uri.EscapeDataString(doc.Numero)}/download-pdf");
        http.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var resp = await _httpClient.SendAsync(http, cancellationToken);
        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        if (!resp.IsSuccessStatusCode)
            return FactusArchivoResult.Failure($"Error Factus ({(int)resp.StatusCode}): {body}");

        using var json = System.Text.Json.JsonDocument.Parse(body);
        var data = json.RootElement.TryGetProperty("data", out var d) ? d : json.RootElement;
        var base64 = data.TryGetProperty("pdf_base_64_encoded", out var b) ? b.GetString() : null;
        var fileName = data.TryGetProperty("file_name", out var fn) ? fn.GetString() : $"factura-{doc.Numero}.pdf";
        if (string.IsNullOrEmpty(base64))
            return FactusArchivoResult.Failure("Respuesta sin contenido PDF.");

        return FactusArchivoResult.Success(Convert.FromBase64String(base64), fileName ?? $"factura-{doc.Numero}.pdf", "application/pdf");
    }

    public async Task<FactusArchivoResult> DescargarXmlAsync(int documentoId, CancellationToken cancellationToken = default)
    {
        var doc = await _context.DocumentosElectronicos.FirstOrDefaultAsync(d => d.Id == documentoId, cancellationToken);
        if (doc is null || string.IsNullOrEmpty(doc.Numero))
            return FactusArchivoResult.Failure("Documento no encontrado o sin número de factura.");

        await _rateLimiter.WaitAsync(cancellationToken);
        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        using var http = new HttpRequestMessage(HttpMethod.Get, $"/v2/bills/{Uri.EscapeDataString(doc.Numero)}/download-xml/");
        http.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var resp = await _httpClient.SendAsync(http, cancellationToken);
        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        if (!resp.IsSuccessStatusCode)
            return FactusArchivoResult.Failure($"Error Factus ({(int)resp.StatusCode}): {body}");

        using var json = System.Text.Json.JsonDocument.Parse(body);
        var data = json.RootElement.TryGetProperty("data", out var d) ? d : json.RootElement;
        var base64 = data.TryGetProperty("xml_base_64_encoded", out var b) ? b.GetString() : null;
        var fileName = data.TryGetProperty("file_name", out var fn) ? fn.GetString() : $"factura-{doc.Numero}.xml";
        if (string.IsNullOrEmpty(base64))
            return FactusArchivoResult.Failure("Respuesta sin contenido XML.");

        return FactusArchivoResult.Success(Convert.FromBase64String(base64), fileName ?? $"factura-{doc.Numero}.xml", "application/xml");
    }
}

public sealed record FactusEmissionResult(bool IsSuccess, string? InvoiceNumber, string? Cufe, string? QrUrl, string? Error)
{
    public static FactusEmissionResult Success(string? invoiceNumber, string? cufe, string? qrUrl) => new(true, invoiceNumber, cufe, qrUrl, null);
    public static FactusEmissionResult Failure(string error) => new(false, null, null, null, error);
}


internal sealed class FactusDocumentoExistente
{
    public bool IsValidated { get; set; }
    public string? Number { get; set; }
    public string? Cufe { get; set; }
    public string? QrUrl { get; set; }
}

public sealed record FactusArchivoResult(bool IsSuccess, byte[]? Bytes, string? FileName, string? ContentType, string? Error)
{
    public static FactusArchivoResult Success(byte[] bytes, string fileName, string contentType) => new(true, bytes, fileName, contentType, null);
    public static FactusArchivoResult Failure(string error) => new(false, null, null, null, error);
}
