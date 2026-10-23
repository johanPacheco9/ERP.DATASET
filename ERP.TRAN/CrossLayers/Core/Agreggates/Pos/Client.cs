using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;

namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public DniType DniType { get; set; } = DniType.cc;
    public string IdentificationNumber { get; set; } = null!;
    public string? Dv { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "Colombia";
    public string? Fax { get; set; }
    public string? LegalOrganizationType { get; set; }
    public string? TaxRegime { get; set; }

    // === FISCAL (Factus / DIAN) ===
    public int? LegalOrganizationCode { get; set; }   // 1 jurídica, 2 natural
    public string? TributeCode { get; set; }            // 01 IVA, ZZ no aplica
    public string? MunicipalityCode { get; set; }      // código Factus
    public string? Responsibilities { get; set; }      // ej. R-99-PN (separados por coma)
}
