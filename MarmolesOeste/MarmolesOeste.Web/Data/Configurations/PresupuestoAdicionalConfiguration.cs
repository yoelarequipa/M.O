using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class PresupuestoAdicionalConfiguration : IEntityTypeConfiguration<PresupuestoAdicional>
{
    public void Configure(EntityTypeBuilder<PresupuestoAdicional> builder)
    {
        builder.Property(p => p.NombreAdicionalCongelado).IsRequired().HasMaxLength(150);
        builder.Property(p => p.PrecioAdicionalCongelado).HasPrecision(18, 2);
        builder.Property(p => p.Cantidad).HasPrecision(18, 2);

        builder
            .HasOne(p => p.Presupuesto)
            .WithMany(pr => pr.Adicionales)
            .HasForeignKey(p => p.PresupuestoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Adicional)
            .WithMany(a => a.PresupuestoAdicionales)
            .HasForeignKey(p => p.AdicionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
