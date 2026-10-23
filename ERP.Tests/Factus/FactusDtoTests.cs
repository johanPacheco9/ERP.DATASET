using System.Globalization;
using System.Text.Json;
using ERP.DATA.Services.FactusService;
using Xunit;

namespace ERP.Tests.Factus;

public class FactusDtoTests
{
    [Fact]
    public void FactusBillRequest_SerializesSnakeCase_AndOmitsNulls()
    {
        var req = new FactusBillRequest
        {
            ReferenceCode = "FACT-1",
            Customer = new FactusCustomer { Identification = "123", IdentificationDocumentCode = "13" },
            Items = new List<FactusItem>
            {
                new FactusItem { CodeReference = "P1", Name = "Prod", Quantity = "1.00", Price = "10000.00" }
            },
            PaymentDetails = new List<FactusPaymentDetail>
            {
                new FactusPaymentDetail { Amount = "10000.00" }
            }
        };

        var json = JsonSerializer.Serialize(req, FactusJson.Options);
        Assert.Contains("\"reference_code\"", json);
        Assert.Contains("\"customer\"", json);
        Assert.DoesNotContain("\"cash_rounding_amount\"", json);
        Assert.DoesNotContain("\"trade_name\"", json);
        Assert.DoesNotContain("ReferenceCode", json);
    }

    [Fact]
    public void Montos_UsanInvariantCulture()
    {
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("es-CO");
            var req = new FactusBillRequest
            {
                ReferenceCode = "X",
                Customer = new FactusCustomer { Identification = "1", IdentificationDocumentCode = "13" },
                Items = new List<FactusItem>
                {
                    new FactusItem
                    {
                        CodeReference = "P",
                        Name = "P",
                        Quantity = (1.5m).ToString("0.00", CultureInfo.InvariantCulture),
                        Price = (1234.5m).ToString("0.00", CultureInfo.InvariantCulture)
                    }
                },
                PaymentDetails = new List<FactusPaymentDetail> { new FactusPaymentDetail { Amount = (1234.5m).ToString("0.00", CultureInfo.InvariantCulture) } }
            };
            var json = JsonSerializer.Serialize(req, FactusJson.Options);
            Assert.Contains("1234.50", json);
            Assert.DoesNotContain("1234,50", json);
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
        }
    }

    [Fact]
    public void FactusValidationException_ExponeErrors()
    {
        var ex = new FactusValidationException("invalid", new[] { "e1", "e2" });
        Assert.Equal(2, ex.Errors.Count);
        Assert.Contains("e1", ex.Errors);
    }

    [Fact]
    public void FactusConflictException_GuardaReferenciaEIsValidated()
    {
        var ex = new FactusConflictException("conflict", "FACT-1", true);
        Assert.Equal("FACT-1", ex.ReferenceCode);
        Assert.True(ex.IsValidated);
    }
}
