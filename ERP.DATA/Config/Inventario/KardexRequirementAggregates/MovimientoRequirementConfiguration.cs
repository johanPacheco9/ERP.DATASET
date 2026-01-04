using ERP.TRAN.CrossLayers.Core.Agreggates.Pos.Inventario.BodegasInventary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.DATA.Config.Inventario.KardexRequirementAggregates;

public class MovimientoRequirementConfiguration : IEntityTypeConfiguration<Movimiento>
{
    public void Configure(EntityTypeBuilder<Movimiento> builder)
    {
        builder.ToTable("Movimientos", "Inventario");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();

        // 🔗 Campos requeridos
        builder.Property(m => m.ProductoVarianteId)
            .IsRequired();

        builder.Property(m => m.BodegaId)
            .IsRequired();

        // Tipo de movimiento (enum → int)
        builder.Property(m => m.TipoMovimiento)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.Cantidad)
            .IsRequired();

        builder.Property(m => m.CostoUnitario)
            .IsRequired()
            .HasColumnType("DECIMAL(15,4)");

        // 💡 Calculado en memoria
        builder.Ignore(m => m.CostoTotal);

        // Referencias externas
        builder.Property(m => m.ReferenciaId);

        builder.Property(m => m.ReferenciaTipo)
            .HasMaxLength(50);

        // Lote / vencimiento
        builder.Property(m => m.Lote)
            .HasMaxLength(100);

        builder.Property(m => m.FechaVencimiento);

        // Motivo / observaciones
        builder.Property(m => m.Motivo)
            .HasMaxLength(500);

        builder.Property(m => m.Observaciones)
            .HasMaxLength(2000);

        // Auditoría
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.CreatedBy)
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        // Índices
        builder.HasIndex(m => new { m.ProductoVarianteId, m.UpdatedAt })
            .HasDatabaseName("IX_Movimientos_ProductoVariante_Fecha");

        builder.HasIndex(m => new { m.BodegaId, m.UpdatedAt })
            .HasDatabaseName("IX_Movimientos_Bodega_Fecha");

        builder.HasIndex(m => m.TipoMovimiento)
            .HasDatabaseName("IX_Movimientos_Tipo");

        builder.HasIndex(m => m.ReferenciaId)
            .HasDatabaseName("IX_Movimientos_ReferenciaId");

        // Relaciones
        builder.HasOne(m => m.ProductoVariante)
            .WithMany(pv => pv.Movimientos)
            .HasForeignKey(m => m.ProductoVarianteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Bodega)
            .WithMany(b => b.Movimiento)
            .HasForeignKey(m => m.BodegaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
