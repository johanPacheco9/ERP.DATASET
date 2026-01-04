using ERP.TRAN.CrossLayers.API.Inventario.Producto.Enums;
using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.ProductosInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.TRAN.CrossLayers.Infrastructure.Data.Configurations.Inventario.ProductosRequirementAggregates
{
    public class ProductoRequirementConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToTable("Productos", "Inventario");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            // 🧾 Propiedades principales
            builder.Property(p => p.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Descripcion)
                .HasMaxLength(500);

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasConversion<int>() // Guarda como int en la BD
                .HasDefaultValue(ProductoEnumStatus.Activo);


            // 💰 Costos y precios
            builder.Property(p => p.Costo_Unitario).HasPrecision(18, 2).HasDefaultValue(0);
            builder.Property(p => p.Precio_Venta).HasPrecision(18, 2).HasDefaultValue(0);

            // 💸 Impuestos
            builder.Property(p => p.PorcentajeIVA).HasPrecision(5, 4).HasDefaultValue(0.19m);
            builder.Property(p => p.PorcentajeICA).HasPrecision(5, 4).HasDefaultValue(0);
            builder.Property(p => p.ImpuestoEspecifico).HasPrecision(18, 2).HasDefaultValue(0);
            builder.Property(p => p.ArancelImportacion).HasPrecision(18, 2).HasDefaultValue(0);

            // ⚖️ Categorización fiscal
            builder.Property(p => p.ExentoIVA).HasDefaultValue(false);
            builder.Property(p => p.GravadoICA).HasDefaultValue(false);
            builder.Property(p => p.CodigoTributario).HasMaxLength(50);

            // 📏 Unidad de medida
            builder.Property(p => p.Unidad_Medida)
                .HasMaxLength(30)
                .HasDefaultValue("unidades");

            // ⚙️ Atributos físicos
            builder.Property(p => p.Peso).HasPrecision(18, 3).HasDefaultValue(0);
            builder.Property(p => p.Volumen).HasPrecision(18, 3).HasDefaultValue(0);
            builder.Property(p => p.Dimensiones).HasMaxLength(100);

            builder.Property(p => p.Es_Perecedero);

            // 🧩 Metadatos
            builder.Property(p => p.Imagen_Url).HasMaxLength(300);
            builder.Property(p => p.Notas).HasMaxLength(1000);
            builder.Property(p => p.Tags).HasMaxLength(200);

            // 🧱 Relaciones
            builder.HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Proveedor)
                .WithMany()
                .HasForeignKey(p => p.ProveedorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.Variantes)
                .WithOne(v => v.Producto)
                .HasForeignKey(v => v.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.StockEnBodegas)
                .WithOne(sb => sb.Producto)
                .HasForeignKey(sb => sb.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🕓 Auditoría
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // ⚡ Índices
            builder.HasIndex(p => p.Codigo).IsUnique();
            builder.HasIndex(p => p.Nombre);
        }
    }
}
