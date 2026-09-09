using System.Text;
using ERP.DATA.Bogus;
using ERP.DATA.DependencyInjections;
using ERP.DATA.Services.TokenService;
using ERP.DATASET.Components;
using ERP.TRAN.CrossLayers.API.Users.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Traceability;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

var builder = WebApplication.CreateBuilder(args);

//APIKEYRESEND = re_4SrwWJ5k_FSeFSpfvFpUP3b368UZrDMQH

// 1. Componentes de Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

builder.Services.AddCascadingAuthenticationState();

// 2. Tus servicios del ERP (Registrados como Transient)
builder.Services.AddDataServices();

// 2.1 Autenticación / JWT
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<TokenManager>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("auth_token", out var token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationCore();

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
    
    if (!await context.Usuarios.AnyAsync())
    {
        var hasher = new PasswordHasher<object>();
        var admin = new Usuario
        {
            PrimerNombre = "Johan",
            PrimerAPellido = "Pacheco",
            Email = "admin@dataset-software.com",
            Role = UserRole.Admin,
            IsActive = true
        };
        admin.PasswordHash = hasher.HashPassword(null!, "CambiaEsta123!");

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorPages();

app.MapMethods("/logout", new[] { "GET", "POST" }, (HttpContext httpContext) =>
{
    httpContext.Response.Cookies.Delete("auth_token", new CookieOptions
    {
        Secure = true,
        SameSite = SameSiteMode.Strict
    });
    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();