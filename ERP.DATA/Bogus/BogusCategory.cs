using Bogus;
using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnitProduct.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using Microsoft.EntityFrameworkCore;
using MainDataContext = ERP.DATA.Repositories.MainDataContext;

namespace ERP.DATA.Bogus;

public static class OneShotDatabaseSeeder
{
    public static async Task SeedAsync(MainDataContext context)
    {
        var faker = new Faker("es");
        var now = DateTime.UtcNow;

        // =========================
        // CATEGORIES
        // =========================
        if (!context.Category.Any())
        {
            var categories = faker.Commerce.Categories(10)
                .Select((name, i) => new Category
                {
                    Name = name,
                    Code = $"CAT-{i + 1}",
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                }).ToList();

            context.Category.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // =========================
        // STORES
        // =========================
        if (!context.Store.Any())
        {
            var stores = Enumerable.Range(1, 2)
                .Select(i => new Store
                {
                    Name = i == 1 ? "Main Store" : $"Store {i}",
                    IsMainStore = i == 1,
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                }).ToList();

            context.Store.AddRange(stores);
            await context.SaveChangesAsync();
        }

        // =========================
        // WAREHOUSES
        // =========================
        if (!context.Warehouse.Any())
        {
            var seedWarehouses = Enumerable.Range(1, 5)
                .Select(i => new Warehouse
                {
                    Code = $"WH-{i:000}",
                    Name = $"Bodega {faker.Address.City()}",
                    Ubication = faker.Address.FullAddress(),
                    Description = faker.Commerce.Department(),
                    Type = WarehouseType.Principal,
                    Max_Capacity = faker.Random.Decimal(1000, 5000),
                    StoreId = 1,
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                }).ToList();

            context.Warehouse.AddRange(seedWarehouses);
            await context.SaveChangesAsync();
        }

        // =========================
        // SUPPLIERS
        // =========================
        if (!context.Supplier.Any())
        {
            var suppliers = Enumerable.Range(1, 5)
                .Select(_ => new Supplier
                {
                    Name = faker.Company.CompanyName(),
                    Email = faker.Internet.Email(),
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                }).ToList();

            context.Supplier.AddRange(suppliers);
            await context.SaveChangesAsync();
        }

        // =========================
        // CLIENTS
        // =========================
        if (!context.Clients.Any())
        {
            var clients = Enumerable.Range(1, 15)
                .Select(i => new ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Client
                {
                    Name = faker.Name.FullName(),
                    DniType = ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType.cc,
                    IdentificationNumber = faker.Random.Replace("##########"),
                    PhoneNumber = faker.Phone.PhoneNumber(),
                    City = faker.Address.City(),
                    Address = faker.Address.StreetAddress()
                }).ToList();

            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();
        }

        // =========================
        // PRODUCTS
        // =========================
        if (!context.LineaProductos.Any())
        {
            var categories = await context.Category.ToListAsync();
            var suppliers = await context.Supplier.ToListAsync();

            var products = Enumerable.Range(1, 100)
                .Select(i => new LineaProducto
                {
                    Code = $"PROD-{i:000}",
                    Name = faker.Commerce.ProductName(),
                    CategoryId  = faker.PickRandom(categories).Id,
                    SupplierId = faker.PickRandom(suppliers).Id,
                    CostoUnitario = faker.Random.Decimal(10, 100),
                    PrecioVenta= faker.Random.Decimal(120, 200),
                    UnidadMedida = "Unidad",
                    Status = LineaProductoStatus.Active,
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                }).ToList();

            context.LineaProductos.AddRange(products);
            await context.SaveChangesAsync();
        }

        // =========================
        // VARIANTS
        // =========================
        var warehouses = await context.Warehouse.ToListAsync();

        if (!context.Productos.Any(p => string.IsNullOrEmpty(p.Serial)))
        {
            var lineas = await context.LineaProductos.ToListAsync();

            var variants = lineas.SelectMany(p =>
                Enumerable.Range(1, 2).Select(i => new Producto
                {
                    LineaProductoId = p.Id,
                    BodegaId = faker.PickRandom(warehouses).Id,
                    SKU = $"{p.Code}-V{i}",
                    PrecioVenta = faker.Random.Decimal(150, 300),
                    CostoUnitario = faker.Random.Decimal(80, 150),
                    Status = ProductoStatus.Available,
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                })
            ).ToList();

            context.Productos.AddRange(variants);
            await context.SaveChangesAsync();
        }

        // =========================
        // UNIT PRODUCTS (SERIALS)
        // =========================
        if (!context.Productos.Any(p => p.Serial != null))
        {
            var lineas = await context.LineaProductos.ToListAsync();

            var unitProducts = Enumerable.Range(1, 200)
                .Select(i =>
                {
                    var linea = faker.PickRandom(lineas);
                    return new Producto
                    {
                        LineaProductoId = linea.Id,
                        BodegaId = faker.PickRandom(warehouses).Id,
                        SKU = $"{linea.Code}-U{i:D4}",
                        Serial = $"SN-{i:D5}",
                        PrecioVenta = faker.Random.Decimal(150, 300),
                        CostoUnitario = faker.Random.Decimal(80, 150),
                        Status = ProductoStatus.Available,
                        CreatedBy = "system",
                        CreatedAt = now,
                        IsActive = true
                    };
                }).ToList();

            context.Productos.AddRange(unitProducts);
            await context.SaveChangesAsync();
        }
    }
}