using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests
{
    public sealed class DeleteProveedorByIdRequest : IValidatableRequest
    {
        [DisplayName("Id del producto a eliminar")]
        [Required(ErrorMessage = "El Id es necesario")]
        public int Id { get; init; }

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
}
