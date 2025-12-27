using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;

public sealed class GetCategoriaByIdRequest : IValidatableRequest
{
    [FromRoute(Name = "id")]
    [Required(ErrorMessage = "El Id de la categoría es obligatorio.")]
    public int Id { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        errors = null;

        if (Id == 0)
        {
            errors = "El Id no puede estar vacío.";
            return false;
        }

        return true;
    }
}

