using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Request;

public class GetUnitProductByCodeRequets : BaseListRequest
{
    public string Code { get; set; }

    public override bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (string.IsNullOrEmpty(Code))
        {
            errors = "El codigo del producto es obligatorio.";
            return false;
        }
        return true;
    }
}