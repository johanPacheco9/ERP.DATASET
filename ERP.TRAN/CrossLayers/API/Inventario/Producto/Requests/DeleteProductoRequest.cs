using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.API.Inventario.Producto.Requests;

public sealed class DeleteProveedorRequest : IValidatableRequest
{
    [DisplayName("Id del producto a eliminar")]
    [Required(ErrorMessage = "El Id es necesario")]
    public Guid Id { get; init; }

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

