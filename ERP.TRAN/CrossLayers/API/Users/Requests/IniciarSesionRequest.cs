namespace ERP.TRAN.CrossLayers.API.Users.Requests;

public sealed class IniciarSesionRequest
{
    public required string Usuario { get; init; }
    public required string Password { get; init; }
}
