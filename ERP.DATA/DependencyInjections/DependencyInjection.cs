using ERP.DATA.Services.Inventario.ProductoService;
using ERP.DATA.Services.Inventario.ProveedorService;
using ERP.DATA.Services.InventarioService.BodeegaService;
using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.DATA.Services.InventarioService.MovimientoService;
using ERP.DATA.Services.InventarioService.ProductoVarianteService;
using ERP.DATA.Services.InventarioService.UnitProductService;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices; // ✅ Namespace correcto
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IBodegas;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IMovimientos;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductosVariantes;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProveedores;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IUnitProduct;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.DATA.DependencyInjections;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // Servicios que no necesitan configuration
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IBodegaService, BodegaService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IProveedorService, ProveedorService>();
        services.AddTransient<IProductoVarianteService, ProductoVarianteService>();
        services.AddTransient<IMovimientoService, MovimientoService>();
        services.AddTransient<IUnitProductService, UnitProductService>();
        // services.AddScoped<IStockBodegaService, StockBodegaService>();

        return services;
    }
}