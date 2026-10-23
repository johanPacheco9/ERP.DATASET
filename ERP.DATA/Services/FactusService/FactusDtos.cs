using System.Text.Json.Serialization;

namespace ERP.DATA.Services.FactusService;

/// <summary>
/// DTOs tipados para la API de Factus. Serialización en snake_case, omitiendo nulos.
/// </summary>
public sealed class FactusBillRequest
{
    [JsonPropertyName("reference_code")]
    public string ReferenceCode { get; set; } = string.Empty;

    [JsonPropertyName("document")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Document { get; set; } = "01";

    [JsonPropertyName("numbering_range_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NumberingRangeId { get; set; }

    [JsonPropertyName("operation_type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OperationType { get; set; } = "10";

    [JsonPropertyName("observation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Observation { get; set; }

    [JsonPropertyName("send_email")]
    public bool SendEmail { get; set; } = true;

    [JsonPropertyName("customer")]
    public FactusCustomer Customer { get; set; } = new();

    [JsonPropertyName("items")]
    public List<FactusItem> Items { get; set; } = new();

    [JsonPropertyName("payment_details")]
    public List<FactusPaymentDetail> PaymentDetails { get; set; } = new();

    [JsonPropertyName("cash_rounding_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CashRoundingAmount { get; set; }

    [JsonPropertyName("prepayment_details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<FactusPrepaymentDetail>? PrepaymentDetails { get; set; }

    [JsonPropertyName("allowance_charges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<FactusAllowanceCharge>? AllowanceCharges { get; set; }

    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FactusCurrency? Currency { get; set; }

    [JsonPropertyName("establishment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FactusEstablishment? Establishment { get; set; }

    [JsonPropertyName("billing_period")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FactusBillingPeriod? BillingPeriod { get; set; }

    [JsonPropertyName("order_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FactusOrderReference? OrderReference { get; set; }

    [JsonPropertyName("related_documents")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<FactusRelatedDocument>? RelatedDocuments { get; set; }

    [JsonPropertyName("created_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CreatedTime { get; set; }
}

public sealed class FactusCustomer
{
    [JsonPropertyName("identification_document_code")]
    public string IdentificationDocumentCode { get; set; } = string.Empty;

    [JsonPropertyName("identification")]
    public string Identification { get; set; } = string.Empty;

    [JsonPropertyName("dv")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Dv { get; set; }

    [JsonPropertyName("legal_organization_code")]
    public string LegalOrganizationCode { get; set; } = "2";

    [JsonPropertyName("tribute_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TributeCode { get; set; } = "ZZ";

    [JsonPropertyName("responsibilities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Responsibilities { get; set; }

    [JsonPropertyName("company")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Company { get; set; }

    [JsonPropertyName("trade_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TradeName { get; set; }

    [JsonPropertyName("names")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Names { get; set; }

    [JsonPropertyName("address")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address { get; set; }

    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; set; }

    [JsonPropertyName("country_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CountryCode { get; set; } = "CO";

    [JsonPropertyName("municipality_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MunicipalityCode { get; set; }
}

public sealed class FactusItem
{
    [JsonPropertyName("code_reference")]
    public string CodeReference { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public string Quantity { get; set; } = "1.00";

    [JsonPropertyName("discount_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DiscountRate { get; set; }

    [JsonPropertyName("discount_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DiscountAmount { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; } = "0.00";

    [JsonPropertyName("unit_measure_code")]
    public string UnitMeasureCode { get; set; } = "94";

    [JsonPropertyName("standard_code")]
    public string StandardCode { get; set; } = "999";

    [JsonPropertyName("taxes")]
    public List<FactusTax> Taxes { get; set; } = new();

    [JsonPropertyName("withholding_taxes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<FactusWithholdingTax>? WithholdingTaxes { get; set; }

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }

    [JsonPropertyName("is_excluded")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsExcluded { get; set; }
}

public sealed class FactusTax
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "01";

    [JsonPropertyName("rate")]
    public string Rate { get; set; } = "0.00";

    [JsonPropertyName("is_excluded")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsExcluded { get; set; }
}

public sealed class FactusWithholdingTax
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("rate")]
    public string Rate { get; set; } = "0.00";
}

public sealed class FactusPaymentDetail
{
    [JsonPropertyName("payment_form")]
    public string PaymentForm { get; set; } = "1";

    [JsonPropertyName("payment_method_code")]
    public string PaymentMethodCode { get; set; } = "10";

    [JsonPropertyName("reference_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReferenceCode { get; set; }

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = "0.00";

    [JsonPropertyName("due_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DueDate { get; set; }
}

public sealed class FactusPrepaymentDetail
{
    [JsonPropertyName("reference_code")]
    public string ReferenceCode { get; set; } = string.Empty;

    [JsonPropertyName("received_date")]
    public string ReceivedDate { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = "0.00";

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }
}

public sealed class FactusAllowanceCharge
{
    [JsonPropertyName("concept_type")]
    public string ConceptType { get; set; } = string.Empty;

    [JsonPropertyName("is_surcharge")]
    public bool IsSurcharge { get; set; }

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("base_amount")]
    public string BaseAmount { get; set; } = "0.00";

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = "0.00";
}

public sealed class FactusCurrency
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("exchange_rate")]
    public string ExchangeRate { get; set; } = string.Empty;
}

public sealed class FactusEstablishment
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("municipality_code")]
    public string MunicipalityCode { get; set; } = string.Empty;
}

public sealed class FactusBillingPeriod
{
    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("start_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StartTime { get; set; }

    [JsonPropertyName("end_date")]
    public string EndDate { get; set; } = string.Empty;

    [JsonPropertyName("end_time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EndTime { get; set; }
}

public sealed class FactusOrderReference
{
    [JsonPropertyName("reference_code")]
    public string ReferenceCode { get; set; } = string.Empty;

    [JsonPropertyName("issue_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IssueDate { get; set; }
}

public sealed class FactusRelatedDocument
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("issue_date")]
    public string IssueDate { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;
}

public sealed class FactusNotaCreditoRequest
{
    [JsonPropertyName("reference_code")]
    public string ReferenceCode { get; set; } = string.Empty;

    [JsonPropertyName("correction_concept_code")]
    public string CorrectionConceptCode { get; set; } = "1";

    [JsonPropertyName("customization_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CustomizationId { get; set; } = "20";

    [JsonPropertyName("bill_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BillNumber { get; set; }

    [JsonPropertyName("numbering_range_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NumberingRangeId { get; set; }

    [JsonPropertyName("observation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Observation { get; set; }

    [JsonPropertyName("customer")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FactusCustomer? Customer { get; set; }

    [JsonPropertyName("items")]
    public List<FactusItem> Items { get; set; } = new();

    [JsonPropertyName("payment_details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<FactusPaymentDetail>? PaymentDetails { get; set; }
}
