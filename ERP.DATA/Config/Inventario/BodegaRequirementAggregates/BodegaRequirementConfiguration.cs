using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BodegaConfig : IEntityTypeConfiguration<Bodega>
{
    public void Configure(EntityTypeBuilder<Bodega> builder)
    {
        builder.ToTable("Bodegas", "Inventario");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.Codigo).IsRequired().HasMaxLength(50);
        builder.Property(b => b.Nombre).IsRequired().HasMaxLength(200);

        // ⭐ AGREGAR ESTO
        builder.Property(b => b.StoreId).IsRequired();

        builder.Property(b => b.Ubicacion).HasMaxLength(500);
        builder.Property(b => b.Descripcion).HasMaxLength(1000);
        builder.Property(b => b.Capacidad_Maxima).HasColumnType("DECIMAL(10,2)");

        // Auditoría
        builder.Property(b => b.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(b => b.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(b => b.UpdatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Índices
        builder.HasIndex(b => b.Codigo).IsUnique();

        // ⭐ ÍNDICE COMPUESTO (código único por tienda)
        builder.HasIndex(b => new { b.StoreId, b.Codigo })
               .IsUnique()
               .HasDatabaseName("IX_Bodegas_Store_Codigo");

        // Relaciones
        builder.HasMany(b => b.StockProductos)
               .WithOne(sb => sb.Bodega)
               .HasForeignKey(sb => sb.BodegaId)
               .OnDelete(DeleteBehavior.Cascade);

        // ⭐ AGREGAR RELACIÓN CON STORE
        builder.HasOne(b => b.Store)
               .WithMany(s => s.Bodegas)
               .HasForeignKey(b => b.StoreId)
               .OnDelete(DeleteBehavior.Restrict); // No borrar Store si tiene bodegas
    }
}