using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class PresupuestoPiezaConfiguration : IEntityTypeConfiguration<PresupuestoPieza>
{
    public void Configure(EntityTypeBuilder<PresupuestoPieza> builder)
    {
        builder.Property(p => p.Profundidad).HasPrecision(18, 2);
        builder.Property(p => p.NombreMaterialCongelado).IsRequired().HasMaxLength(150);
        builder.Property(p => p.PrecioMaterialCongelado).HasPrecision(18, 2);
        builder.Property(p => p.PrecioZocaloCongelado).HasPrecision(18, 2);
        builder.Property(p => p.ColorMaterialCongelado).IsRequired().HasMaxLength(7);
        builder.Property(p => p.NombreTerminacionCongelado).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PrecioTerminacionCongelado).HasPrecision(18, 2);

        builder
            .HasOne(p => p.Presupuesto)
            .WithMany(pr => pr.Piezas)
            .HasForeignKey(p => p.PresupuestoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Material)
            .WithMany(m => m.PresupuestoPiezas)
            .HasForeignKey(p => p.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.TerminacionCanto)
            .WithMany(t => t.PresupuestoPiezas)
            .HasForeignKey(p => p.TerminacionCantoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
