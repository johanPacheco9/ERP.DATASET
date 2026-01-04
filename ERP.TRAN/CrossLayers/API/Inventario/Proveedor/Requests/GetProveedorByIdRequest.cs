using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests;
public sealed class GetProveedorByIdRequest : IValidatableRequest
{
    [FromRoute(Name = "id")]
    [DisplayName("Id Del Proveedor")]
    public  int Id { get; init; }

    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (Id == 0)
        {
            errors = "El Id del producto es obligatorio.";
            return false;
        }
        return true;
    }
}

