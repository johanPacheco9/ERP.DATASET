using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;

public sealed class ListUnitProductAuditsRequest : IValidatableRequest
{
    [FromQuery(Name = "auditId")]
    public int? AuditId { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        return true;
    }
}
