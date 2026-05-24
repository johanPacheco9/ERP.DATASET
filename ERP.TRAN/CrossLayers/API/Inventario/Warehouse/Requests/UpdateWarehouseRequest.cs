using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;

public sealed class UpdateWarehouseRequest : IValidatableRequest
{
    [DisplayName("Id de la bodega a actualizar")]
    [Required]
    public int Id { get; set; }

    [DisplayName("Código a actualizar")]
    public string? Code { get; set; }

    [DisplayName("Código")]
    public decimal? Max_Capacity { get; set; }

    public bool ParametersAreValid(out string? errors)
    {
        errors = null;
        if (Id == 0)   
        {
            errors = "El id de la bodega a actualizar es necesario."; 
            return false;
        }
        return true;
    }
}
