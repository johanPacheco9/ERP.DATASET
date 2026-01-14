using ERP.DATA.DependencyInjections;
using ERP.DATA.Repositories;
using ERP.DATA.Services.Inventario.ProductoService;
using ERP.DATA.Services.InventarioService.CategoriaService;
using ERP.DATA.Services.InventarioService.ProductoVarianteService;
using ERP.DATASET.Components;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.ICategorias;
using ERP.TRAN.CrossLayers.Core.Interfaces.InventarioServices.IProductosVariantes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDataServices();
builder.Services.AddDbContext<MainDataContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
