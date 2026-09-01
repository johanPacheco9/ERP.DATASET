using Bogus;
using ERP.TRAN.CrossLayers.API.Inventario.ProductoVariante.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.UnidadProducto.Enums;
using ERP.TRAN.CrossLayers.API.Inventario.Warehouse.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;
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
        // CATEGORIES (Controladas y coherentes)
        // =========================
        if (!context.Category.Any())
        {
            var categoryNames = new[]
            {
                "Computadores y Laptops", "Smartphones y Telefonía", "Componentes de PC",
                "Periféricos y Accesorios", "Impresión y Escáneres", "Redes y Conectividad",
                "Almacenamiento", "Audio y Video", "Software y Licencias", "Seguridad Electrónica"
            };

            var categories = categoryNames
                .Select((name, i) => new Category
                {
                    Name = name,
                    Code = $"CAT-{i + 1:00}",
                    CreatedBy = 1,
                    CreatedAt = now,
                    IsActive = true
                }).ToList();

            context.Category.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // =========================
        // BRANDS
        // =========================
        if (!context.Marca.Any())
        {
            var brandNames = new[]
            {
                "Samsung", "Apple", "Xiaomi", "Lenovo", "HP",
                "Dell", "Asus", "Sony", "LG", "Logitech"
            };

            var brands = brandNames
                .Select(name => new Marca
                {
                    Nombre = name,
                    Descripcion = $"Fabricante líder en {name}",
                    LogoUrl = null,
                    Activa = true,
                    CreatedBy = 1,
                    CreatedAt = now,
                    IsActive = true
                })
                .ToList();

            context.Marca.AddRange(brands);
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
                    Name = i == 1 ? "Tienda Principal Bucaramanga" : $"Sucursal Norte {i}",
                    IsMainStore = i == 1,
                    CreatedBy = 1,
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
            var seedWarehouses = Enumerable.Range(1, 3)
                .Select(i => new Warehouse
                {
                    Code = $"WH-{i:000}",
                    Name = i == 1 ? "Bodega Central" : $"Bodega Auxiliar {i}",
                    Ubication = $"Zona Industrial - Bodega {i}",
                    Description = "Almacenamiento principal de inventario general",
                    Type = i == 1 ? WarehouseType.Principal : WarehouseType.LossWarehouse,
                    Max_Capacity = 5000m,
                    StoreId = 1,
                    CreatedBy = 1,
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
            var supplierNames = new[] { "TechGlobal S.A.S.", "CompuMarket Colombia", "Distribuidores Mayoristas del Caribe", "Importaciones de Tecnología S.A.", "Soluciones Informáticas Andinas" };
            
            var suppliers = supplierNames
                .Select(name => new Proveedor
                {
                    Name = name,
                    Email = faker.Internet.Email(),
                    CreatedBy = 1,
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
                .Select(_ => new ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Client
                {
                    Name = faker.Name.FullName(),
                    DniType = ERP.TRAN.CrossLayers.API.Pos.Clients.Enums.DniType.cc,
                    IdentificationNumber = faker.Random.Replace("##########"),
                    PhoneNumber = faker.Phone.PhoneNumber(),
                    City = "Bucaramanga",
                    Address = faker.Address.StreetAddress()
                }).ToList();

            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();
        }

        // =========================
        // PRODUCT BASES (CATÁLOGO) - Nombres profesionales
        // =========================
        if (!context.ProductoBase.Any())
        {
            var categories = await context.Category.ToListAsync();
            var brands = await context.Marca.ToListAsync();

            var productAdjectives = new[] { "Pro", "Ultra", "Max", "Slim", "Advanced", "Gaming", "Enterprise", "Essential", "Prime", "Lite" };
            var productItems = new[] { "Laptop", "Monitor", "Teclado Mecánico", "Mouse Inalámbrico", "Disco Estado Sólido SSD", "Memoria RAM", "Router Wi-Fi 6", "Impresora Multifuncional", "Auriculares Gamer", "Tablet" };

            var products = Enumerable.Range(1, 50)
                .Select(i =>
                {
                    var brand = faker.PickRandom(brands);
                    var item = faker.PickRandom(productItems);
                    var adj = faker.PickRandom(productAdjectives);
                    var name = $"{brand.Nombre} {item} {adj}";

                    var product = new ProductoBase
                    {
                        Code = $"PRD-{i:000}",
                        Name = name,
                        MarcaId = brand.Id,
                        CostoUnitario = faker.Random.Decimal(150000, 800000),
                        PrecioVenta = faker.Random.Decimal(250000, 1200000),
                        UnidadMedida = "Unidad",
                        BaseStatus = ProductoBaseStatus.Active,
                        CreatedBy = 1,
                        CreatedAt = now,
                        IsActive = true
                    };

                    // Asignación de 1 o 2 categorías lógicas
                    var selectedCategories = categories
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(faker.Random.Int(1, 2))
                        .ToList();

                    foreach (var cat in selectedCategories)
                    {
                        product.Categorias.Add(new ProductoBaseCategory
                        {
                            CategoryId = cat.Id
                        });
                    }

                    return product;
                })
                .ToList();

            context.ProductoBase.AddRange(products);
            await context.SaveChangesAsync();
        }

        // =========================
        // PRODUCT PROVIDERS
        // =========================
        if (!context.ProductoProveedor.Any())
        {
            var products = await context.ProductoBase.ToListAsync();
            var suppliers = await context.Supplier.ToListAsync();

            var productSuppliers = new List<ProductoProveedor>();

            foreach (var product in products)
            {
                var selectedSuppliers = suppliers
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(faker.Random.Int(1, 2))
                    .ToList();

                for (var i = 0; i < selectedSuppliers.Count; i++)
                {
                    var supplier = selectedSuppliers[i];

                    productSuppliers.Add(new ProductoProveedor
                    {
                        ProductoBaseId = product.Id,
                        ProveedorId = supplier.Id,
                        CostoUnitario = product.CostoUnitario,
                        CodigoProveedor = $"SUP-{supplier.Id}-{product.Code}",
                        DiasEntrega = faker.Random.Int(2, 7),
                        EsPrincipal = i == 0,
                        CreatedBy = 1,
                        CreatedAt = now,
                        IsActive = true
                    });
                }
            }

            context.ProductoProveedor.AddRange(productSuppliers);
            await context.SaveChangesAsync();
        }

        // =========================
        // PRODUCT VARIANTS (SKUs)
        // =========================
        if (!context.ProductoVariantes.Any())
        {
            var bases = await context.ProductoBase.ToListAsync();

            var variants = bases
                .SelectMany(p =>
                    Enumerable.Range(1, 2)
                        .Select(i => new ProductoVariante
                        {
                            ProductoBaseId = p.Id,
                            SKU = $"{p.Code}-V{i}",
                            PrecioVenta = p.PrecioVenta + (i * 50000),
                            CostoUnitario = p.CostoUnitario,
                            Status = ProductoVarianteStatus.Active,
                            CreatedBy = 1,
                            CreatedAt = now,
                            IsActive = true
                        }))
                .ToList();

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

            var unitProducts = Enumerable.Range(1, 100)
                .Select(i =>
                {
                    var variante = faker.PickRandom(variantes);
                    var bodega = faker.PickRandom(warehouses);

                    return new UnidadProducto
                    {
                        ProductoVarianteId = variante.Id,
                        BodegaId = bodega.Id,
                        SerialNumber = $"SN-2026-{i:D5}",
                        Lote = "LOT-2026-01",
                        Status = UnidadProductoStatus.Available,
                        UbicacionFisica = $"Rack-{faker.Random.Number(1, 5)}-Nivel-{faker.Random.Number(1, 3)}",
                        CreatedBy = 1,
                        CreatedAt = now,
                        IsActive = true
                    };
                })
                .ToList();

            context.UnidadesProductos.AddRange(unitProducts);
            await context.SaveChangesAsync();

            var stocks =
                (from v in variantes
                 from w in warehouses
                 select new WarehouseStock
                 {
                     WarehouseId = w.Id,
                     ProductoVarianteId = v.Id,
                     CurrentStock = faker.Random.Number(5, 30),
                     StockReservado = 0,
                     StockMinimo = 2,
                     StockMaximo = 50,
                     FechaActualizacion = now,
                     CreatedBy = 1,
                     CreatedAt = now,
                     IsActive = true
                 })
                .ToList();

            context.WarehouseStock.AddRange(stocks);
            await context.SaveChangesAsync();
        }
    }
}