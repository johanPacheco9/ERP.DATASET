using ERP.DATA.Services.CajaService;
using ERP.DATA.Services.Inventario.ProveedorService;
using ERP.DATA.Services.VentasService.ClientService;
using ERP.DATA.Services.VentasService.Payments;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.DATA.Services.InventarioService.AuditService;
using ERP.DATA.Services.InventarioService.WarehouseService;
using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.DATA.Services.InventarioService.ProductoVarianteService;
using ERP.DATA.Services.InventarioService.UnidadProductoService;
using Microsoft.Extensions.DependencyInjection;
using ProductoBaseService = ERP.DATA.Services.InventarioService.ProductoBaseService.ProductoBaseService;

namespace ERP.DATA.DependencyInjections;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // Todos como Transient: se crean, ejecutan la consulta usando la factoría y se destruyen.
        // Cero consumo innecesario de memoria en el servidor.
        services.AddTransient<AuditoriaService>();
        services.AddTransient<ProductoBaseService>();
        services.AddTransient<WarehouseService>();
        services.AddTransient<CategoriaService>();
        services.AddTransient<SupplierService>();
        services.AddTransient<ProductVariantService>();
        services.AddTransient<MovimientoService>();
        services.AddTransient<UnidadProductoManager>();
        services.AddTransient<ClientService>();
        services.AddTransient<SaleService>();
        services.AddTransient<PaymentsService>();
        services.AddTransient<CajaManager>();

        return services;
    }
}
