using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.TRAN.CrossLayers.Infrastructure.Data.Configurations.Inventario.ProductosRequirementAggregates;
public class CategoriaRequirementConfiguration: IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias", "Inventario");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        builder.Property(c => c.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        // Índices
        builder.HasIndex(c => c.Codigo)
            .IsUnique();

        builder.HasIndex(c => c.Nombre);
    }
}