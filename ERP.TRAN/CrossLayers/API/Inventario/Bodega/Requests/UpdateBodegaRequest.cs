using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;

namespace ERP.TRAN.CrossLayers.API.Inventario.Bodega.Requests;

public sealed class UpdateBodegaRequest : IValidatableRequest
{
    public int Id { get; set; }
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
