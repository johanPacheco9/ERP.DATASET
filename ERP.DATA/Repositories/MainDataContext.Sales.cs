using ERP.TRAN.CrossLayers.Core.Agreggates.Pos;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Sales;
using Microsoft.EntityFrameworkCore;

namespace ERP.DATA.Repositories;

public partial class MainDataContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(e =>
        {
            e.ToTable("Clients");
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.Property(c => c.IdentificationNumber).HasMaxLength(50).IsRequired();
            e.HasIndex(c => c.IdentificationNumber);
        });

        modelBuilder.Entity<Sale>(e =>
        {
            e.ToTable("Sales");
            e.HasKey(s => s.Id);
            e.Property(s => s.SaleNumber).HasMaxLength(30).IsRequired();
            e.HasIndex(s => s.SaleNumber).IsUnique();
            e.HasOne(s => s.Client).WithMany().HasForeignKey(s => s.ClientId);
            e.HasOne(s => s.Warehouse).WithMany().HasForeignKey(s => s.WarehouseId);
            e.HasOne(s => s.Store).WithMany().HasForeignKey(s => s.StoreId);
        });

        modelBuilder.Entity<SaleLineItem>(e =>
        {
            e.ToTable("SaleLineItems");
            e.HasKey(l => l.Id);
            e.HasOne(l => l.Sale).WithMany(s => s.Lines).HasForeignKey(l => l.SaleId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(l => l.LineaProducto).WithMany().HasForeignKey(l => l.LineaProductoId);
            e.HasOne(l => l.Producto).WithMany().HasForeignKey(l => l.ProductoId).OnDelete(DeleteBehavior.SetNull);
        });
    }
}
