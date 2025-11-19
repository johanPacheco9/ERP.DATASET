using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;

public sealed class GetBodegaByIdRequest : IValidatableRequest
{
    [FromRoute(Name = "id")]
    public Guid Id { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        errors = null!;
       if (Id == Guid.Empty)
        {
            errors = "El Id no puede estar vacío.";
            return false;
        }
        return true;
    }
}

