using Bogus;
using ERP.DATA.Repositories;
using ERP.TRAN.CrossLayers.API.Inventario.Bodega.Enums;
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
                .Select(_ => new Proveedor
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
        // PRODUCT BASES (CATÁLOGO)
        // =========================
        if (!context.ProductoBase.Any())
        {
            var categories = await context.Category.ToListAsync();
            var suppliers = await context.Supplier.ToListAsync();

            var products = Enumerable.Range(1, 100)
                .Select(i => new ProductoBase
                {
                    Code = $"PROD-{i:000}",
                    Name = faker.Commerce.ProductName(),
                    CategoryId = faker.PickRandom(categories).Id,
                    SupplierId = faker.PickRandom(suppliers).Id,
                    CostoUnitario = faker.Random.Decimal(10, 100),
                    PrecioVenta = faker.Random.Decimal(120, 200),
                    UnidadMedida = "Unidad",
                    BaseStatus = ProductoBaseStatus.Active,
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                }).ToList();

            context.ProductoBase.AddRange(products);
            await context.SaveChangesAsync();
        }

        // =========================
        // PRODUCT VARIANTS (SKUs)
        // =========================
        if (!context.ProductoVariantes.Any())
        {
            var bases = await context.ProductoBase.ToListAsync();

            var variants = bases.SelectMany(p =>
                Enumerable.Range(1, 2).Select(i => new ProductoVariante
                {
                    ProductoBaseId = p.Id,
                    SKU = $"{p.Code}-V{i}",
                    PrecioVenta = faker.Random.Decimal(150, 300),
                    CostoUnitario = faker.Random.Decimal(80, 150),
                    CreatedBy = "system",
                    CreatedAt = now,
                    IsActive = true
                })
            ).ToList();

            context.ProductoVariantes.AddRange(variants);
            await context.SaveChangesAsync();
        }

        // =========================
        // UNIT PRODUCTS & WAREHOUSE STOCK
        // =========================
        if (!context.UnidadesProductos.Any())
        {
            var warehouses = await context.Warehouse.ToListAsync();
            var variantes = await context.ProductoVariantes.ToListAsync();

            // 1. Crear Unidades Serializadas (para los ítems de alta trazabilidad)
            var unitProducts = Enumerable.Range(1, 200)
                .Select(i =>
                {
                    var variante = faker.PickRandom(variantes);
                    var bodega = faker.PickRandom(warehouses);
                    return new UnidadProducto
                    {
                        ProductoVarianteId = variante.Id,
                        BodegaId = bodega.Id,
                        SerialNumber = $"SN-{i:D5}",
                        Lote = $"LOT-{faker.Random.Number(100, 999)}",
                        Status = UnidadProductoStatus.Available,
                        UbicacionFisica = $"Estante {faker.Random.AlphaNumeric(3).ToUpper()}",
                        CreatedBy = "system",
                        CreatedAt = now,
                        IsActive = true
                    };
                }).ToList();

            context.UnidadesProductos.AddRange(unitProducts);
            await context.SaveChangesAsync();

            // 2. Crear Saldo Inicial en WarehouseStock por cada Variante y Bodega
            var stocks = (from v in variantes
                          from w in warehouses
                          select new WarehouseStock
                          {
                              WarehouseId = w.Id,
                              ProductoVarianteId = v.Id,
                              CurrentStock = faker.Random.Number(10, 100),
                              StockReservado = 0,
                              StockMinimo = 5,
                              StockMaximo = 150,
                              FechaActualizacion = now,
                              CreatedBy = "system",
                              CreatedAt = now,
                              IsActive = true
                          }).ToList();

            context.WarehouseStock.AddRange(stocks);
            await context.SaveChangesAsync();
        }
    }
}