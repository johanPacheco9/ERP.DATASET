using ERP.TRAN.CrossLayers.Core.Utilities.Contracts;
using System.ComponentModel.DataAnnotations;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;

public abstract class BaseUpdateRequest(int updaterId) : IValidatableRequest
{
    [Required(ErrorMessage = "Debe indicar el usuario que actualiza")]
    public int _UpdaterAuth0Id { get; init; } = updaterId;

    public abstract bool ParametersAreValid(out string? errors);
}
