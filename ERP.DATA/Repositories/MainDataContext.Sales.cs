using ERP.TRAN.CrossLayers.Core.Agreggates.Pos;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventory.WarehouseInventory;
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

            // Relación con la Venta (Cascada al eliminar la venta)
            e.HasOne(l => l.Sale)
                .WithMany(s => s.Lines)
                .HasForeignKey(l => l.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación obligatoria con la Variante del Producto
            e.HasOne(l => l.ProductoVariante)
                .WithMany()
                .HasForeignKey(l => l.ProductoVarianteId)
                .OnDelete(DeleteBehavior.Restrict); // Evita borrados accidentales de variantes con ventas históricas

            // Relación opcional con la Unidad Específica (para productos serializados / garantías)
            e.HasOne(l => l.UnidadProducto)
                .WithMany()
                .HasForeignKey(l => l.UnidadProductoId)
                .OnDelete(DeleteBehavior.SetNull);
        });


        modelBuilder.Entity<Movement>(entity =>
        {
            // Configuración para la bodega de origen
            entity.HasOne(m => m.OrigenWarehouse)
                .WithMany()
                .HasForeignKey(m => m.OrigenWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración para la bodega de destino (permite nulos por si es entrada/salida simple)
            entity.HasOne(m => m.DestinationWarehouse)
                .WithMany()
                .HasForeignKey(m => m.DestinationWarehouseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}