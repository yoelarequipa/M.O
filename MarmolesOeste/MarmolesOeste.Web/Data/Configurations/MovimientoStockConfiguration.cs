using MarmolesOeste.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarmolesOeste.Web.Data.Configurations;

public class MovimientoStockConfiguration : IEntityTypeConfiguration<MovimientoStock>
{
    public void Configure(EntityTypeBuilder<MovimientoStock> builder)
    {
        builder.Property(m => m.Cantidad).HasPrecision(18, 2);
        builder.Property(m => m.Motivo).IsRequired().HasMaxLength(300);

        builder
            .HasOne(m => m.Material)
            .WithMany(mat => mat.MovimientosStock)
            .HasForeignKey(m => m.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
