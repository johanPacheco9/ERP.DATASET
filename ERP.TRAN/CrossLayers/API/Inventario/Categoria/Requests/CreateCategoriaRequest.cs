using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Inventario.Categoria.Requests;

public class CreateCategoriaRequest : BaseCreateRequest, IValidatableRequest
{

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 3,ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
    public string? Descripcion { get; set; }

    [StringLength(12, MinimumLength =3, ErrorMessage = "El código debe estar entre 3 y 12 caracteres")]
    public string? codigo { get; set; } 

    public override bool ParametersAreValid(out string? errors)
    {
        if (codigo == null)
        {
            errors = "El campo codigo es requerido";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            errors = "El nombre es obligatorio.";
            return false;
        }



        errors = null;
        return true;
    }
}
