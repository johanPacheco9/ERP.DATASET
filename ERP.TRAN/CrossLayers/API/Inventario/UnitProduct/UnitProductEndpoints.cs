
using ERP.TRAN.CrossLayers.API.Base;

namespace ERP.TRAN.CrossLayers.API.Inventario.UnitProduct;

public static class UnitProductEndpoints
{

    public const string List = "/api/v1/inventario/UnitProducts";

    public const string Get = $"{List}/{PathLiterals.PrimaryKeyPlaceholder}";
    
    public const string GetByBaseCode = $"{List}/GetByBaseCode";
}
