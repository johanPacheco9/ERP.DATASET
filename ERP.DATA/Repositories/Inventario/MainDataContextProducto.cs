using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Repositories;

public partial class MainDataContext
{
    public DbSet<Producto> Productos { get; set; }

    public DbSet<Bodega> Bodegas { get; set; }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Proveedor> Proveedores { get; set; }

    public DbSet<StockBodega> StockBodegas { get; set; }

    public DbSet<Movimiento> Movimientos { get; set; }


}

