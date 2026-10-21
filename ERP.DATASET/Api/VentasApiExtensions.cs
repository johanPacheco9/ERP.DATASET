using System.Security.Claims;
using ERP.DATA.Repositories;
using ERP.DATA.Services.TokenService;
using ERP.DATA.Services.VentasService.SaleService;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Clients.Enums;
using ERP.TRAN.CrossLayers.API.Pos.Sales.Requests;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATASET.Api;

/// <summary>
/// Endpoints HTTP mínimos que exponen el flujo de ventas (POS) para poder
/// ejecutar pruebas de carga con k6. La lógica de negocio vive en
/// <see cref="SaleService"/>; aquí solo se traduce HTTP -> servicio.
/// </summary>
/// <remarks>
/// Todos los endpoints de ventas exigen autenticación (la política fallback del
/// proyecto). k6 se autentica enviando la cookie <c>auth_token</c>.
/// Los endpoints bajo <c>/api/v1/dev/*</c> solo se montan en Development.
/// </remarks>
public static class VentasApiExtensions
{
    /// <summary>
    /// Mapea los endpoints de ventas y (en Development) los de apoyo para las pruebas.
    /// </summary>
    public static WebApplication MapVentasApi(this WebApplication app)
    {
        // =====================================================================
        // VENTAS (requieren autenticación)
        // =====================================================================

        // POST /api/v1/pos/ventas -> crea una venta
        app.MapPost("/api/v1/pos/ventas", async (
                CreateSaleRequest request,
                ClaimsPrincipal user,
                SaleService sales,
                CancellationToken cancellationToken) =>
            {
                var userId = GetUserId(user);
                if (userId is null)
                    return Results.Unauthorized();

                // El servicio usa este campo como metadato de auditoría.
                request._CreatorAuth0Id = userId.Value;

                var result = await sales.CreateAsync(request, cancellationToken);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/pos/ventas/{result.Value.Id}", result.Value)
                    : Results.BadRequest(new { code = result.Error.Code, message = result.Error.Message });
            })
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithName("CrearVenta")
            .WithTags("POS - Ventas");

        // GET /api/v1/pos/ventas -> lista paginada
        app.MapGet("/api/v1/pos/ventas", async (
                int? pageNumber,
                int? pageSize,
                string? search,
                int? warehouseId,
                SaleService sales,
                CancellationToken cancellationToken) =>
            {
                var request = new ListSalesRequest
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = pageSize ?? 20,
                    Search = search,
                    WarehouseId = warehouseId
                };

                var paged = await sales.ListPagedAsync(request, cancellationToken);
                return Results.Ok(paged);
            })
            .RequireAuthorization()
            .WithName("ListarVentas")
            .WithTags("POS - Ventas");

        // POST /api/v1/pos/ventas/{id}/anular -> anula la venta completa
        app.MapPost("/api/v1/pos/ventas/{id:int}/anular", async (
                int id,
                CancelSaleRequest body,
                ClaimsPrincipal user,
                SaleService sales,
                CancellationToken cancellationToken) =>
            {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var result = await sales.CancelSaleAsync(id, body.Motivo, userId.Value, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { code = result.Error.Code, message = result.Error.Message });
            })
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithName("AnularVenta")
            .WithTags("POS - Ventas");

        // POST /api/v1/pos/ventas/{id}/devoluciones -> devolución parcial de una línea
        app.MapPost("/api/v1/pos/ventas/{id:int}/devoluciones", async (
                int id,
                ReturnLineRequest body,
                ClaimsPrincipal user,
                SaleService sales,
                CancellationToken cancellationToken) =>
            {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var result = await sales.ReturnLineAsync(id, body.LineId, body.Quantity, body.SerialNumber, body.Motivo, userId.Value, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { code = result.Error.Code, message = result.Error.Message });
            })
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithName("DevolverLineaVenta")
            .WithTags("POS - Ventas");

        // =====================================================================
        // APOYO A LAS PRUEBAS (solo Development)
        // =====================================================================
        if (app.Environment.IsDevelopment())
        {
            // POST /api/v1/dev/token -> devuelve un JWT para un usuario semilla.
            // Evita a k6 tener que lidiar con antiforgery y cookies Secure por HTTPS.
            app.MapPost("/api/v1/dev/token", async (
                    DevTokenRequest body,
                    MainDataContext db,
                    TokenManager tokens) =>
                {
                    var usuario = await db.Usuarios
                        .FirstOrDefaultAsync(u => u.Email == body.Email && u.IsActive);

                    if (usuario is null)
                        return Results.Unauthorized();

                    var hasher = new PasswordHasher<object>();
                    var verificacion = hasher.VerifyHashedPassword(null!, usuario.PasswordHash, body.Password);

                    if (verificacion == PasswordVerificationResult.Failed)
                        return Results.Unauthorized();

                    var token = tokens.GenerarToken(usuario.Id, usuario.UserName, usuario.Role);
                    return Results.Ok(new { token, userId = usuario.Id, email = usuario.Email });
                })
                .AllowAnonymous()
                .DisableAntiforgery()
                .WithName("DevToken")
                .WithTags("Dev");

            // POST /api/v1/dev/ventas/prepare?stock=1000000&serialUnits=2000
            // Deja listo el escenario de prueba con los DOS caminos de venta:
            //  - noSerial: variante sin serial, con stock agregado alto.
            //    La venta descuenta WarehouseStock y consume capas FIFO de Movement.
            //  - serial:   variante que REQUIERE serial, con N unidades físicas
            //    disponibles (y stock agregado = N). La venta exige SerialNumber y,
            //    con ForzarFifoEstricto, debe despachar la unidad más antigua.
            app.MapPost("/api/v1/dev/ventas/prepare", async (
                    int? stock,
                    int? serialUnits,
                    MainDataContext db,
                    CancellationToken cancellationToken) =>
                {
                    var stockObjetivo = stock ?? 1_000_000;
                    var unidadesObjetivo = serialUnits ?? 2_000;

                    var sitio = await AsegurarSitioAsync(db, cancellationToken);
                    var (store, warehouse, client) = (sitio.Store, sitio.Warehouse, sitio.Client);

                    // ---------- Variante SIN serial ----------
                    var noSerial = await AsegurarVarianteAsync(
                        db,
                        requiereSerial: false,
                        code: "PRD-LOAD",
                        name: "Producto Prueba Carga",
                        costo: 10_000m,
                        precio: 20_000m,
                        cancellationToken);

                    await PonerStockAsync(db, warehouse.Id, noSerial.Id, stockObjetivo, cancellationToken);

                    // ---------- Variante CON serial + unidades físicas ----------
                    var serial = await AsegurarVarianteAsync(
                        db,
                        requiereSerial: true,
                        code: "PRD-LT-SERIAL",
                        name: "Producto Prueba Serial (FIFO)",
                        costo: 15_000m,
                        precio: 30_000m,
                        cancellationToken);

                    var disponibles = await db.UnidadesProductos.CountAsync(
                        u => u.BodegaId == warehouse.Id &&
                             u.ProductoVarianteId == serial.Id &&
                             u.Status == UnidadProductoStatus.Available,
                        cancellationToken);

                    if (disponibles < unidadesObjetivo)
                    {
                        var faltan = unidadesObjetivo - disponibles;
                        var base_ = DateTime.UtcNow;

                        for (var i = 0; i < faltan; i++)
                        {
                            db.UnidadesProductos.Add(new UnidadProducto
                            {
                                ProductoVarianteId = serial.Id,
                                BodegaId = warehouse.Id,
                                SerialNumber = $"LT-{serial.Id}-{Guid.NewGuid():N}",
                                Lote = "LOTE-LT",
                                Status = UnidadProductoStatus.Available,
                                UbicacionFisica = "Rack-LT",
                                CreatedBy = 1,
                                // CreatedAt creciente y distinto => orden FIFO determinista.
                                CreatedAt = base_.AddMilliseconds(disponibles + i)
                            });
                        }

                        await db.SaveChangesAsync(cancellationToken);
                        disponibles += faltan;
                    }

                    // El stock agregado del serializado debe reflejar las unidades disponibles.
                    await PonerStockAsync(db, warehouse.Id, serial.Id, disponibles, cancellationToken);

                    return Results.Ok(new
                    {
                        storeId = store.Id,
                        warehouseId = warehouse.Id,
                        clientId = client.Id,
                        noSerial = new
                        {
                            productoVarianteId = noSerial.Id,
                            unitPrice = Precio(noSerial),
                            taxRate = Iva(noSerial),
                            stock = stockObjetivo
                        },
                        serial = new
                        {
                            productoVarianteId = serial.Id,
                            unitPrice = Precio(serial),
                            taxRate = Iva(serial),
                            availableUnits = disponibles
                        }
                    });
                })
                .AllowAnonymous()
                .DisableAntiforgery()
                .WithName("DevPrepararVentas")
                .WithTags("Dev");

            // GET /api/v1/dev/ventas/next-serial?warehouseId=1&productoVarianteId=2
            // Devuelve la unidad física DISPONIBLE más antigua (el "siguiente" en
            // FIFO estricto) para una variante serializada en una bodega.
            app.MapGet("/api/v1/dev/ventas/next-serial", async (
                    int warehouseId,
                    int productoVarianteId,
                    MainDataContext db,
                    CancellationToken cancellationToken) =>
                {
                    var unidad = await db.UnidadesProductos
                        .Where(u => u.BodegaId == warehouseId &&
                                    u.ProductoVarianteId == productoVarianteId &&
                                    u.Status == UnidadProductoStatus.Available)
                        .OrderBy(u => u.CreatedAt)
                        .ThenBy(u => u.Id)
                        .Select(u => new { serialNumber = u.SerialNumber, unidadProductoId = u.Id })
                        .FirstOrDefaultAsync(cancellationToken);

                    // 200 con serialNumber=null cuando ya no quedan unidades (no usa 404
                    // para no contar como fallo en las métricas de k6).
                    return Results.Ok(new
                    {
                        serialNumber = unidad != null ? unidad.serialNumber : null,
                        unidadProductoId = unidad != null ? unidad.unidadProductoId : (int?)null
                    });
                })
                .AllowAnonymous()
                .WithName("DevNextSerial")
                .WithTags("Dev");
        }

        return app;
    }

    // ------------------------------------------------------------------------
    // Helpers de preparación de datos
    // ------------------------------------------------------------------------

    private static async Task<(Store Store, Warehouse Warehouse, ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Client Client)>
        AsegurarSitioAsync(MainDataContext db, CancellationToken cancellationToken)
    {
        var ahora = DateTime.UtcNow;

        var store = await db.Store.FirstOrDefaultAsync(s => s.IsMainStore, cancellationToken)
                    ?? await db.Store.FirstOrDefaultAsync(cancellationToken);
        if (store is null)
        {
            store = new Store { Name = "Tienda Carga", IsMainStore = true, CreatedBy = 1, CreatedAt = ahora };
            db.Store.Add(store);
            await db.SaveChangesAsync(cancellationToken);
        }

        var warehouse = await db.Warehouse.FirstOrDefaultAsync(w => w.StoreId == store.Id, cancellationToken);
        if (warehouse is null)
        {
            warehouse = new Warehouse
            {
                Code = "WH-LOAD",
                Name = "Bodega Carga",
                Type = WarehouseType.Principal,
                StoreId = store.Id,
                CreatedBy = 1,
                CreatedAt = ahora
            };
            db.Warehouse.Add(warehouse);
            await db.SaveChangesAsync(cancellationToken);
        }

        var client = await db.Clients.FirstOrDefaultAsync(cancellationToken);
        if (client is null)
        {
            client = new ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Client
            {
                Name = "CLIENTE PRUEBA CARGA",
                DniType = DniType.cc,
                IdentificationNumber = "2222222222",
                City = "Bucaramanga"
            };
            db.Clients.Add(client);
            await db.SaveChangesAsync(cancellationToken);
        }

        return (store, warehouse, client);
    }

    private static async Task<ProductoVariante> AsegurarVarianteAsync(
        MainDataContext db,
        bool requiereSerial,
        string code,
        string name,
        decimal costo,
        decimal precio,
        CancellationToken cancellationToken)
    {
        // Para el caso sin serial, reutiliza cualquier variante del seed no serializada.
        var variante = requiereSerial
            ? await db.ProductoVariantes.Include(v => v.ProductoBase).FirstOrDefaultAsync(v => v.ProductoBase.RequiereSerial, cancellationToken)
            : await db.ProductoVariantes.Include(v => v.ProductoBase).FirstOrDefaultAsync(v => !v.ProductoBase.RequiereSerial, cancellationToken);

        if (variante is not null)
            return variante;

        var ahora = DateTime.UtcNow;

        var marca = await db.Marca.FirstOrDefaultAsync(cancellationToken);
        if (marca is null)
        {
            marca = new Marca { Nombre = "Marca Carga", Activa = true, CreatedBy = 1, CreatedAt = ahora };
            db.Marca.Add(marca);
            await db.SaveChangesAsync(cancellationToken);
        }

        var baseProducto = new ProductoBase
        {
            Code = code,
            Name = name,
            MarcaId = marca.Id,
            CostoUnitario = costo,
            PrecioVenta = precio,
            UnidadMedida = "Unidad",
            RequiereSerial = requiereSerial,
            BaseStatus = ProductoBaseStatus.Active,
            CreatedBy = 1,
            CreatedAt = ahora
        };
        db.ProductoBase.Add(baseProducto);
        await db.SaveChangesAsync(cancellationToken);

        variante = new ProductoVariante
        {
            ProductoBaseId = baseProducto.Id,
            SKU = $"{code}-V1",
            PrecioVenta = baseProducto.PrecioVenta,
            CostoUnitario = baseProducto.CostoUnitario,
            Status = ProductoVarianteStatus.Active,
            CreatedBy = 1,
            CreatedAt = ahora
        };
        db.ProductoVariantes.Add(variante);
        await db.SaveChangesAsync(cancellationToken);
        variante.ProductoBase = baseProducto;
        return variante;
    }

    private static async Task PonerStockAsync(
        MainDataContext db,
        int warehouseId,
        int productoVarianteId,
        int cantidad,
        CancellationToken cancellationToken)
    {
        var ahora = DateTime.UtcNow;

        var stockRow = await db.WarehouseStock
            .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId &&
                                      s.ProductoVarianteId == productoVarianteId, cancellationToken);
        if (stockRow is null)
        {
            db.WarehouseStock.Add(new WarehouseStock
            {
                WarehouseId = warehouseId,
                ProductoVarianteId = productoVarianteId,
                CurrentStock = cantidad,
                StockReservado = 0,
                StockMinimo = 0,
                StockMaximo = cantidad,
                FechaActualizacion = ahora,
                CreatedBy = 1,
                CreatedAt = ahora
            });
        }
        else
        {
            stockRow.CurrentStock = cantidad;
            stockRow.FechaActualizacion = ahora;
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static decimal Precio(ProductoVariante variante)
        => variante.PrecioVenta ?? variante.ProductoBase.PrecioVenta;

    private static decimal Iva(ProductoVariante variante)
        => variante.ProductoBase.ExentoIVA ? 0m : variante.ProductoBase.PorcentajeIVA;

    private static int? GetUserId(ClaimsPrincipal user)
    {
        var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var id) ? id : null;
    }

    /// <summary>Cuerpo del endpoint de login de desarrollo.</summary>
    public sealed record DevTokenRequest(string Email, string Password);

    public sealed record CancelSaleRequest(string Motivo);
    public sealed record ReturnLineRequest(int LineId, int Quantity, string? SerialNumber, string Motivo);
}
