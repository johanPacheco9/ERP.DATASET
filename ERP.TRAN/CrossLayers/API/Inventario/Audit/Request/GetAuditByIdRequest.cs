using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;

public sealed class GetAuditByIdRequest : IValidatableRequest
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (Id <= 0)
        {
            errors = "El Id debe ser mayor a cero.";
            return false;
        }
        return true;
    }
}
