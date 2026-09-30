using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class PresupuestoPiezaTramoConfiguration : IEntityTypeConfiguration<PresupuestoPiezaTramo>
{
    public void Configure(EntityTypeBuilder<PresupuestoPiezaTramo> builder)
    {
        builder.Property(t => t.Largo).HasPrecision(18, 2);

        builder
            .HasOne(t => t.PresupuestoPieza)
            .WithMany(p => p.Tramos)
            .HasForeignKey(t => t.PresupuestoPiezaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
