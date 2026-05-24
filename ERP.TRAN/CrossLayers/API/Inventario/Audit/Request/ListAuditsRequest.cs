using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;

namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;

public sealed class ListAuditsRequest : IValidatableRequest
{
    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        return true;
    }
}
