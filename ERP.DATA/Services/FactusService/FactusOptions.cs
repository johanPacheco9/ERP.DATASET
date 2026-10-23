namespace ERP.DATA.Services.FactusService;

/// <summary>
/// Configuración de la integración con la API de Factus (facturación electrónica Colombia).
/// Se enlaza desde la sección "Factus" de appsettings.json.
/// </summary>
public class FactusOptions
{
    public string BaseUrl { get; set; } = "https://api-sandbox.factus.com.co";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? NumberingRangeId { get; set; }
    public bool SendEmail { get; set; } = true;
}
