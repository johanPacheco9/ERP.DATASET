using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;
namespace ERP.TRAN.CrossLayers.Core.Agreggates.Pos;
public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public DniType DniType { get; set; }
    public string IdentificationNumber { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Region { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public string? Fax { get; set; }
}

