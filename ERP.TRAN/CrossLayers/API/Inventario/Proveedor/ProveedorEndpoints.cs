using ERP.TRAN.CrossLayers.API.Base;

namespace ERP.TRAN.CrossLayers.API.Inventario.Proveedor;
public static class ProveedorEndpoints
{
    public const string List = "/api/v1/inventario/Supplier";

    public const string Get = $"{List}/{PathLiterals.PrimaryKeyPlaceholder}";

}

