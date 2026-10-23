using System.Text.Json;
using System.Text.Json.Serialization;

namespace ERP.DATA.Services.FactusService;

public static class FactusJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };
}
