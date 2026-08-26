using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Pos.Terminals.Requests;

public sealed class CreateTerminalRequest : BaseCreateRequest
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int StoreId { get; set; } = 1;
    public int WarehouseId { get; set; } = 1;
    public string Prefix { get; set; } = "POS1";
    public string? DianResolutionNumber { get; set; }
    public string? DianResolutionDate { get; set; }
    public long FromNumber { get; set; } = 1;
    public long ToNumber { get; set; } = 999999;

    public override bool ParametersAreValid(out string? errors)
    {
        var list = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            list.Add("El nombre de la caja es obligatorio.");
        if (string.IsNullOrWhiteSpace(Code))
            list.Add("El código de la caja es obligatorio.");
        if (string.IsNullOrWhiteSpace(Prefix))
            list.Add("El prefijo DIAN es obligatorio.");
        errors = list.Any() ? string.Join("; ", list) : null;
        return errors == null;
    }
}
