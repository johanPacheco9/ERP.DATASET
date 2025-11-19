
using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;
public sealed class GetProductoByIdRequest : IValidatableRequest
{
    [FromRoute(Name = "id")]
    [DisplayName("Id Del Producto")]

    public  Guid Id { get; init; }

    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (Id == Guid.Empty)
        {
            errors = "El Id del producto es obligatorio.";
            return false;
        }
        return true;
    }
}

