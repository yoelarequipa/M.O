using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class PresupuestoPiezaCaladoConfiguration : IEntityTypeConfiguration<PresupuestoPiezaCalado>
{
    public void Configure(EntityTypeBuilder<PresupuestoPiezaCalado> builder)
    {
        builder.Property(c => c.Ancho).HasPrecision(18, 2);
        builder.Property(c => c.Fondo).HasPrecision(18, 2);
        builder.Property(c => c.DistanciaLateral).HasPrecision(18, 2);
        builder.Property(c => c.DistanciaFrontal).HasPrecision(18, 2);

        builder
            .HasOne(c => c.PresupuestoPieza)
            .WithMany(p => p.Calados)
            .HasForeignKey(c => c.PresupuestoPiezaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
