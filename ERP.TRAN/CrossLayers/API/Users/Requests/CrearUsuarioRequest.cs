using ERP.TRAN.CrossLayers.API.Users.Enums;
namespace ERP.TRAN.CrossLayers.API.Users.Requests;

public sealed class CrearUsuarioRequest
{
    public required string PrimerNombre { get; set; }
    public string? SegundoNombre { get; set; }
    public required string PrimerApellido { get; set; }
    public string? SegundoApellido { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }
    public int? StoreId { get; set; }
}