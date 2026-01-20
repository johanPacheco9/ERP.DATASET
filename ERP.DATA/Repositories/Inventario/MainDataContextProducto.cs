using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.AuditoriasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.DATA.Repositories;

public partial class MainDataContext
{
    public DbSet<Producto> Productos { get; set; }

    public DbSet<Bodega> Bodegas { get; set; }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Proveedor> Proveedores { get; set; }

    public DbSet<StockBodega> StockBodegas { get; set; }

    public DbSet<Movimiento> Movimientos { get; set; }

    public DbSet<UnitProductAudit> AuditoriaProductos{get; set;}

    public DbSet<ProductoAuditado> productosAuditados { get; set; }

    public DbSet<ProductoVariante> ProductoVariantes { get; set; }

    public DbSet<UnitProduct> UnitProduct { get; set; }

    public DbSet<UnitProductMovement> UnitProductMovements { get; set; }
}

