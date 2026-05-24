using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;

namespace ERP.TRAN.CrossLayers.API.Pos.Clients.Responses;

public sealed record ClientSummaryDto(
    int Id,
    string Name,
    DniType DniType,
    string IdentificationNumber,
    string? PhoneNumber,
    string? City
);
