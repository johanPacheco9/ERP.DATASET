using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Proveedor.Requests
{
    public sealed class CreateProveedorRequest : IValidatableRequest
    {
        /// <summary>
        /// Nombre del proveedor
        /// </summary>
        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [MaxLength(150, ErrorMessage = "El nombre no puede tener más de 150 caracteres.")]
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// NIT o identificación tributaria
        /// </summary>
        [MaxLength(20, ErrorMessage = "El NIT no puede superar los 20 caracteres.")]
        public string? Nit { get; set; }

        /// <summary>
        /// Dirección física del proveedor
        /// </summary>
        [MaxLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
        public string? Direccion { get; set; }

        /// <summary>
        /// Teléfono o contacto principal
        /// </summary>
        [MaxLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Telefono { get; set; }

        /// <summary>
        /// Estado del proveedor (activo o inactivo)
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Validaciones personalizadas
        /// </summary>
        public bool ParametersAreValid(out string? errors)
        {
            errors = null;

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                errors = "El nombre del proveedor es obligatorio.";
                return false;
            }

            if (!string.IsNullOrEmpty(Nit) && Nit.Length > 20)
            {
                errors = "El NIT no puede tener más de 20 caracteres.";
                return false;
            }

            if (!string.IsNullOrEmpty(Direccion) && Direccion.Length > 200)
            {
                errors = "La dirección no puede tener más de 200 caracteres.";
                return false;
            }

            if (!string.IsNullOrEmpty(Telefono) && Telefono.Length > 20)
            {
                errors = "El teléfono no puede tener más de 20 caracteres.";
                return false;
            }

            return true;
        }
    }
}
