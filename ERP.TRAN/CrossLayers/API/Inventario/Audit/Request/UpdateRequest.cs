using ERP.TRAN.CrossLayers.Core.Utilities.Base.Requests;

namespace ERP.TRAN.CrossLayers.API.Inventario.Audit.Request;

public sealed class UpdateProductoRequest(int updaterId)
    : BaseUpdateRequest(updaterId)
{
    public string Detalles { get; set; } = null!;
    public string Status { get; set; } = null!;

    public override bool ParametersAreValid(out string? errors)
    {
        if (string.IsNullOrWhiteSpace(Detalles))
        {
            errors = "El Detalle de la auditoría es obligatorio.";
            return false;
        }

        if (string.IsNullOrEmpty(Status))
        {
            errors = "El estado no puede ser nulo.";
            return false;
        }

        errors = null;
        return true;
    }
}
