namespace ERP.DATASET.Components.Pages;

public partial class Home
{
    private List<StatCard> Stats = new()
    {
        new("Ventas del Mes", "$124,500", "+12.5%", true),
        new("Productos en Stock", "1,234", "-3.2%", false),
        new("Órdenes Pendientes", "87", "+8.1%", true),
        new("Clientes Activos", "456", "+15.3%", true),
    };

    private List<Order> Orders = new()
    {
        new("ORD-001","Juan Pérez","Laptop Dell","$1,200","Completado"),
        new("ORD-002","María García","iPhone 13","$999","Pendiente"),
        new("ORD-003","Carlos López","Monitor LG","$450","Completado"),
    };

    private string GetEstadoClass(string estado) =>
        estado switch
        {
            "Completado" => "bg-success",
            "Pendiente" => "bg-warning text-dark",
            _ => "bg-primary"
        };
}
record StatCard(string Title, string Value, string Change, bool IsPositive);
record Order(string Id, string Cliente, string Producto, string Monto, string Estado);