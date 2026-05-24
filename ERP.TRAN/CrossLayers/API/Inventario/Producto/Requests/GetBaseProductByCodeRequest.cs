using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;

public class GetBaseProductByCodeRequest : IValidatableRequest
{
    public string Code { get; set; }
    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (string.IsNullOrEmpty(Code))
        {
            errors = "El código del producto es requerido";
        }
        return ParametersAreValid(out errors);
    }
}