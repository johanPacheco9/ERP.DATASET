namespace ERP.TRAN.CrossLayers.Core.Agreggates;

public class Parametros
{
    public int Id { get; set; }
    public string Nit { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Logo1 { get; set; }
    
    // Flags de control operacional para el ERP
    public bool ForzarFifoEstricto { get; set; } = false;
    public bool PermitirVentaSinStock { get; set; } = false;
    public bool ManejoUbicacionesBodega { get; set; } = false;
}