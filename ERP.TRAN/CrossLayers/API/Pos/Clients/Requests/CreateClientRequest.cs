using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Pos.Clients.Requests;

public sealed class CreateClientRequest : BaseCreateRequest
{
    public string Name { get; set; } = null!;
    public DniType DniType { get; set; }
    public string IdentificationNumber { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            list.Add("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(IdentificationNumber))
            list.Add("El número de identificación es obligatorio.");
        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}
