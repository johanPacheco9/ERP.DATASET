using ERP.TRAN.CrossLayers.Core.Agreggates.Pos;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.AuditoriasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.ProductsInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.Stores;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.UnitProducts;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using Microsoft.EntityFrameworkCore;
namespace ERP.DATA.Repositories;

public partial class MainDataContext
{
    public DbSet<ProductoBase> ProductoBase { get; set; }

    public DbSet<Warehouse> Warehouse { get; set; }

    public DbSet<Category> Category { get; set; }

    public DbSet<Proveedor> Supplier { get; set; }

    public DbSet<WarehouseStock> WarehouseStock { get; set; }

    public DbSet<Movement> Movements { get; set; }

    public DbSet<UnidadProductoAuditada> UnitProductAudits{get; set;}
    
    public DbSet<ProductoVariante> ProductoVariantes { get; set; }

    public DbSet<UnidadProducto> UnidadesProductos { get; set; }

    public DbSet<UnitProductMovement> UnitProductMovements { get; set; }

    public DbSet<Audit> Audit { get; set; }

    public DbSet<Store> Store { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Sale> Sales { get; set; }

    public DbSet<SaleLineItem> SaleLineItems { get; set; }
    
}

