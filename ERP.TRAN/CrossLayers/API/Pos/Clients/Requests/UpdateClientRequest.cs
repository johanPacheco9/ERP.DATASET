using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;

namespace ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;

public sealed class UpdateClientRequest
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
    public string? TaxRegime { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (Id <= 0)
            list.Add("El cliente es obligatorio.");
        if (string.IsNullOrWhiteSpace(Name))
            list.Add("El nombre o razón social es obligatorio.");
        if (string.IsNullOrWhiteSpace(IdentificationNumber))
            list.Add("El número de identificación es obligatorio.");

        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}