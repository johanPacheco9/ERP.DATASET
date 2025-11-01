using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.BodegasInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ERP.DATA.Config.Inventario.BodegaRequirementAggregates;

public class BodegaConfig : IEntityTypeConfiguration<Bodega>
{
    public void Configure(EntityTypeBuilder<Bodega> builder)
    {
        builder.ToTable("Bodegas", "Inventario");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .ValueGeneratedOnAdd();

        builder.Property(b => b.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Ubicacion)
            .HasMaxLength(500);

        builder.Property(b => b.Descripcion)
            .HasMaxLength(1000);

        builder.Property(b => b.Capacidad_Maxima)
            .HasColumnType("DECIMAL(10,2)");

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Índices
        builder.HasIndex(b => b.Codigo)
            .IsUnique();

        // Relaciones
        builder.HasMany(b => b.StockProductos)
            .WithOne(sb => sb.Bodega)
            .HasForeignKey(sb => sb.BodegaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


