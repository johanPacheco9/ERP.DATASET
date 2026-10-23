using System.Net.Http.Json;
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
    private readonly FactusOptions _options;
    private readonly MainDataContext _context;

    public FactusService(HttpClient httpClient, IFactusTokenProvider tokenProvider, IOptions<FactusOptions> options, MainDataContext context)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _options = options.Value;
        _context = context;
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

        if (sale.FactusStatus == "Validada" || sale.FactusStatus == "Emitida a DIAN")
            return FactusEmissionResult.Failure("La venta ya fue emitida a Factus.");

        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);

        var request = BuildRequest(sale);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v2/bills/validate")
        {
            Content = JsonContent.Create(request, options: FactusJson.Options)
        };
        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            // 409 Conflict: factura pendiente por enviar a DIAN -> se debe eliminar y reintentar.
            return FactusEmissionResult.Failure($"Error Factus ({(int)response.StatusCode}): {body}");
        }

        // Parsear respuesta mínima (Factus devuelve data.bill con number, cufe, qr, etc.)
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(body);
            var root = doc.RootElement;
            var data = root.TryGetProperty("data", out var d) ? d : root;
            var bill = data.TryGetProperty("bill", out var b) ? b : data;

            sale.FactusInvoiceNumber = bill.TryGetProperty("number", out var n) ? n.GetString() : sale.FactusInvoiceNumber;
            sale.FactusCufe = bill.TryGetProperty("cufe", out var c) ? c.GetString() : sale.FactusCufe;
            sale.FactusQrUrl = bill.TryGetProperty("qr", out var q) ? q.GetString() : sale.FactusQrUrl;
            sale.FactusStatus = "Validada";

            await _context.SaveChangesAsync(cancellationToken);

            return FactusEmissionResult.Success(sale.FactusInvoiceNumber, sale.FactusCufe, sale.FactusQrUrl);
        }
        catch (Exception ex)
        {
            return FactusEmissionResult.Failure($"Respuesta de Factus no parseable: {ex.Message}. Body: {body}");
        }
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
}

public sealed record FactusEmissionResult(bool IsSuccess, string? InvoiceNumber, string? Cufe, string? QrUrl, string? Error)
{
    public static FactusEmissionResult Success(string? invoiceNumber, string? cufe, string? qrUrl) => new(true, invoiceNumber, cufe, qrUrl, null);
    public static FactusEmissionResult Failure(string error) => new(false, null, null, null, error);
}

