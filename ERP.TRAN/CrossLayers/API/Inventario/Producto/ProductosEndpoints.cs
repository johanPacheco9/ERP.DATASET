using ERP.TRAN.CrossLayers.API.Base;

namespace ERP.TRAN.CrossLayers.API.Inventario.Producto;
public static class ProductEndpoints
{
    public const string List = "/api/v1/inventario/Productos";

    public const string Get = $"{List}/{PathLiterals.PrimaryKeyPlaceholder}";

}

