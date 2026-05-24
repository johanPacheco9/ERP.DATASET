using ERP.DATA.Bogus;
using ERP.DATA.DependencyInjections;
using ERP.DATA.Repositories;
using ERP.DATASET.Components;
using Microsoft.EntityFrameworkCore;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

var builder = WebApplication.CreateBuilder(args);

// 1. Componentes de Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. Tus servicios del ERP (Registrados como Transient)
builder.Services.AddDataServices();

// 3. CAMBIO CLAVE: Cambiamos a DbContextFactory para blindar Blazor Server
builder.Services.AddDbContextFactory<MainDataContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// 4. FIX DEL SEEDER: Adaptado para usar la factoría en Desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    // En lugar de pedir el contexto directo, le pedimos la factoría instalada
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MainDataContext>>();
    
    // Creamos un contexto temporal exclusivo para migrar y poblar la base de datos
    using var context = await contextFactory.CreateDbContextAsync();
    
    await context.Database.MigrateAsync();
    await OneShotDatabaseSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();