using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.Property(v => v.Total).HasPrecision(18, 2);

        builder
            .HasOne(v => v.Presupuesto)
            .WithOne(p => p.Venta)
            .HasForeignKey<Venta>(v => v.PresupuestoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
