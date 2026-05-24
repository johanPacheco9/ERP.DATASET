using ERP.TRAN.CrossLayers.API.Base;

namespace ERP.TRAN.CrossLayers.API.Inventario.Audit;

public static class AuditEndpoints
{
    public const string List = "/api/v1/inventario/Audits";

    public const string Get = $"{List}/{PathLiterals.PrimaryKeyPlaceholder}";

    public const string UnitProductAudits = $"{List}/unit-product-audits";

    public const string UnitProductAuditById = $"{UnitProductAudits}/{PathLiterals.PrimaryKeyPlaceholder}";
}
