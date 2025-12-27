using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;

public sealed class CreateBodegaRequest : IValidatableRequest
{
    [Required(ErrorMessage = "El nombre de la bodega es requerido")]
    [DisplayName("Nombre bodega.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El Código de la bodega es requerido.")]
    [StringLength(10, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 10 caracteres.")]
    public string Code { get; set; } = null!;

    [DisplayName("Descripcion de la bodega.")]
    [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La ubicación de la bodega es requerida.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "La ubicación debe tener entre 3 y 150 caracteres.")]
    public string Ubicacion { get; set; } = null!;


    [Range(1, int.MaxValue, ErrorMessage = "La capacidad máxima debe ser un número positivo.")]
    public int CapacidadMaxima { get; set; }


    public bool EsActiva { get; set; } = true;

    public WarehouseType TipoBodega { get; set; }

    public int storeId { get; set; }
    
    public bool ParametersAreValid(out string? errors)
    {
        errors = null!;
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            errors = "El nombre es obligatorio.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(Ubicacion))
        {
            errors = "La ubicación es obligatoria.";
            return false;
        }
        if (CapacidadMaxima <= 0)
        {
            errors = "La capacidad máxima debe ser un número positivo.";
            return false;
        }

        if(string.IsNullOrWhiteSpace(Code))
        {
            errors = "El código es obligatorio.";
            return false;
        }
        return true;
    }
}

