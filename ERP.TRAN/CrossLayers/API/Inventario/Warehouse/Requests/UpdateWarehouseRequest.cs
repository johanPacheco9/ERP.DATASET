using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;

namespace ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Requests;

public sealed class UpdateWarehouseRequest : IValidatableRequest
{
    [DisplayName("Id de la bodega a actualizar")]
    [Required]
    public int Id { get; set; }

    [DisplayName("Código a actualizar")]
    public string? Code { get; set; }

    [DisplayName("Código")]
    public decimal? Max_Capacity { get; set; }
    
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public string? Ubication { get; set; }
    
    //Por si llega a ser asignada a otra tienda.
    
    public int? StoreId { get; set; }
    
    public WarehouseType Type { get; set; }
    
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
