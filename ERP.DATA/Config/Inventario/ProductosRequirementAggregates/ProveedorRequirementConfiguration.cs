using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.TRAN.CrossLayers.Infrastructure.Data.Configurations.Inventario.ProductosRequirementAggregates
{
    public class ProveedorRequirementConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("Proveedores", "Inventario");

            // 🔹 Clave primaria
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            // 🔹 Propiedades principales
            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Nit)
                .HasMaxLength(20);

            builder.Property(p => p.Direccion)
                .HasMaxLength(500);

            builder.Property(p => p.Telefono)
                .HasMaxLength(20);

            builder.Property(p => p.Email)
                .HasMaxLength(150);

            builder.Property(p => p.Activo)
                .HasDefaultValue(true);

            // 🔹 Propiedades heredadas (traceability)
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            builder.Property(p => p.CreatedBy)
                .HasMaxLength(100);

            builder.Property(p => p.UpdatedBy)
                .HasMaxLength(100);

            // 🔹 Relaciones
            builder.HasMany(p => p.Productos)
                .WithOne(p => p.Proveedor)
                .HasForeignKey(p => p.ProveedorId)
                .OnDelete(DeleteBehavior.SetNull);

            // 🔹 Índices
            builder.HasIndex(p => p.Nit)
                .IsUnique()
                .HasFilter("[Nit] IS NOT NULL");

            builder.HasIndex(p => p.Activo);
        }
    }
}

