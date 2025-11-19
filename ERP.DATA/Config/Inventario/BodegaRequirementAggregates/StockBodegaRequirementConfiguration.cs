using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockBodegaRequirementConfiguration : IEntityTypeConfiguration<StockBodega>
{
    public void Configure(EntityTypeBuilder<StockBodega> builder)
    {
        builder.ToTable("StockBodegas", "Inventario");

        builder.HasKey(sb => sb.Id);
        builder.Property(sb => sb.Id).ValueGeneratedOnAdd();

        builder.Property(sb => sb.ProductoVarianteId).IsRequired(false);
        builder.Property(sb => sb.ProductoId).IsRequired(false);
        builder.Property(sb => sb.BodegaId).IsRequired();

        builder.Property(sb => sb.StockActual).HasDefaultValue(0);
        builder.Property(sb => sb.StockMinimo).HasDefaultValue(0);
        builder.Property(sb => sb.StockMaximo).HasDefaultValue(0);
        builder.Property(sb => sb.StockReservado).HasDefaultValue(0);
        builder.Property(sb => sb.FechaActualizacion).HasDefaultValueSql("GETUTCDATE()");

        // 🔹 Índice único cuando existe una variante
        builder.HasIndex(sb => new { sb.ProductoVarianteId, sb.BodegaId })
            .IsUnique()
            .HasDatabaseName("IX_StockBodegas_ProductoVarianteId_BodegaId");

        // 🔹 Índice único cuando el producto no tiene variantes
        builder.HasIndex(sb => new { sb.ProductoId, sb.BodegaId })
            .IsUnique()
            .HasDatabaseName("IX_StockBodegas_ProductoId_BodegaId");

        // 🔗 Relaciones
        builder.HasOne(sb => sb.ProductoVariante)
            .WithMany(pv => pv.StockEnBodegas)
            .HasForeignKey(sb => sb.ProductoVarianteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sb => sb.Producto)
            .WithMany(p => p.StockEnBodegas)
            .HasForeignKey(sb => sb.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sb => sb.Bodega)
            .WithMany(b => b.StockProductos)
            .HasForeignKey(sb => sb.BodegaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
