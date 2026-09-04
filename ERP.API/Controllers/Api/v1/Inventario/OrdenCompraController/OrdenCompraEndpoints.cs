namespace ERP.API.Controllers.Api.v1.Inventario.OrdenCompraController;

public static class OrdenCompraEndpoints
{
    public const string Create = "api/v1/inventario/compras";
    public const string List = "api/v1/inventario/compras";
    public const string GetById = "api/v1/inventario/compras/{id:int}";
    public const string Aprobar = "api/v1/inventario/compras/{id:int}/aprobar";
    public const string Enviar = "api/v1/inventario/compras/{id:int}/enviar";
    public const string Cancelar = "api/v1/inventario/compras/{id:int}/cancelar";
}
