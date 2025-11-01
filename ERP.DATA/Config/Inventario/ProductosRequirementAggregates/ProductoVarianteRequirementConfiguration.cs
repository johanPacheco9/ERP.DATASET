using ERP.TRAN.CrossLayers.Core.Agreggates.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.DATA.Config.Inventario.ProductosRequirementAggregates
{
    public class ProductoVarianteRequirementConfiguration : IEntityTypeConfiguration<ProductoVariante>
    {
        public void Configure(EntityTypeBuilder<ProductoVariante> builder)
        {
            builder.ToTable("ProductoVariantes", "Inventario");

            // 🔑 Clave primaria
            builder.HasKey(pv => pv.Id);
            builder.Property(pv => pv.Id)
                .ValueGeneratedOnAdd();

            // 🧩 Relación con Producto
            builder.HasOne(pv => pv.Producto)
                .WithMany(p => p.Variantes)
                .HasForeignKey(pv => pv.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🧩 Relación con Movimientos
            builder.HasMany(pv => pv.Movimientos)
                .WithOne(m => m.ProductoVariante)
                .HasForeignKey(m => m.ProductoVarianteId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🧩 Relación con StockBodegas
            builder.HasMany(pv => pv.StockEnBodegas)
                .WithOne(sb => sb.ProductoVariante)
                .HasForeignKey(sb => sb.ProductoVarianteId)
                .OnDelete(DeleteBehavior.Cascade);

            // 📦 Propiedades principales
            builder.Property(pv => pv.CodigoVariante)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(pv => pv.CodigoVariante)
                .IsUnique()
                .HasDatabaseName("IX_ProductoVariantes_CodigoVariante");

            builder.Property(pv => pv.Codigo_Barras)
                .HasMaxLength(100);

            builder.Property(pv => pv.Lote)
                .HasMaxLength(100);

            // 📅 Fecha de vencimiento
            builder.Property(pv => pv.Fecha_Vencimiento)
                .HasColumnType("datetime2");

            // 💰 Costos y precios
            builder.Property(pv => pv.Precio_Venta)
                .HasPrecision(18, 2);

            builder.Property(pv => pv.Costo_Unitario)
                .HasPrecision(18, 2);

            // 🧬 Atributos dinámicos
            builder.Property(pv => pv.Atributos)
                .HasColumnType("nvarchar(max)");

            // 🕓 Auditoría
            builder.Property(pv => pv.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(pv => pv.UpdatedAt)
                .HasConversion<DateTime?>(
                    v => v,
                    v => v == null ? null : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                );
        }
    }
}
